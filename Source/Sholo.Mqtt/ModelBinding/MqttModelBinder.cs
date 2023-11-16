using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Utilities;

namespace Sholo.Mqtt.ModelBinding;

public class MqttModelBinder : IMqttModelBinder
{
    public MqttModelBinder()
    {
    }

    public bool TryPerformModelBinding(
        IMqttRequestContext requestContext,
        IMqttTopicFilter topicPatternFilter,
        MethodInfo action,
        [MaybeNullWhen(false)] out IDictionary<ParameterInfo, object?> actionArguments)
    {
        var logger = requestContext.ServiceProvider.GetService<ILogger<RouteProvider>>();

        // See if the request message's topic matches the pattern & extract arguments
        if (!topicPatternFilter.IsMatch(requestContext, out var topicArguments))
        {
            actionArguments = null;
            return false;
        }

        var actionParameters = action.GetParameters();

        var modelBindingContext = new MqttModelBindingContext(
            action,
            topicPatternFilter.TopicPattern,
            requestContext,
            topicArguments!,
            logger
        );

        // Attempt to bind the topic arguments, services, etc. to the action/method parameters
        if (!TryBindActionParameters(modelBindingContext, out actionArguments))
        {
            return false;
        }

        // Handle the case where we couldn't match 100% of the action arguments to parameters
        if (actionArguments.Count != actionParameters.Length)
        {
            var unmatchedParameters = string.Join(", ", actionArguments.Keys.Except(actionParameters).Select(x => x.Name));

            logger?.LogDebug(
                "Evaluating candidate handler {TopicPattern}: Failed to bind the following parameters: {UnmatchedParameters}",
                topicPatternFilter.TopicPattern,
                unmatchedParameters
            );
            return false;
        }

        logger?.LogDebug(
            "Executing {TopicPattern}",
            topicPatternFilter.TopicPattern
        );

        return true;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindActionParameters(IMqttModelBindingContext mqttModelBindingContext, out IDictionary<ParameterInfo, object?> actionArguments)
    {
        actionArguments = new Dictionary<ParameterInfo, object?>();

        ParameterInfo? unboundParameter = null;
        var actionParameters = mqttModelBindingContext.Action.GetParameters();

        foreach (var actionParameter in actionParameters)
        {
            if (TryBindCancellationToken(mqttModelBindingContext, actionArguments, actionParameter) ||
                TryBindParameter(mqttModelBindingContext, actionArguments, actionParameter) ||
                TryBindCorrelationData(mqttModelBindingContext, actionArguments, actionParameter) ||
                TryBindUserProperty(mqttModelBindingContext, actionArguments, actionParameter) ||
                TryBindService(mqttModelBindingContext, actionArguments, actionParameter))
            {
                continue;
            }

            if (unboundParameter != null)
            {
                // There can only be 1 unmatched parameter (the one that might be the payload)
                return false;
            }

            // Need to hold the slot. We'll supply the value below.
            actionArguments.Add(actionParameter, null);
            unboundParameter = actionParameter;
        }

        if (unboundParameter == null)
        {
            mqttModelBindingContext.Logger?.LogWarning("No remaining parameters available for payload binding");
            return false;
        }

        var payloadParameter = unboundParameter;
        if (!TryBindPayload(mqttModelBindingContext, payloadParameter, out var payload))
        {
            mqttModelBindingContext.Logger?.LogWarning("Failed to bind payload");
            return false;
        }

        if (!ValidationHelper.IsValid(payload!, out var validationResults))
        {
            mqttModelBindingContext.Logger?.LogWarning(
                "The message payload failed validation:{NewLine}{ValidationErrors}",
                Environment.NewLine,
                string.Join(Environment.NewLine, validationResults.Select(x => $"{x.ErrorMessage} ({string.Join(", ", x.MemberNames)})"))
            );
            return false;
        }

        actionArguments[payloadParameter] = payload;
        return true;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindCancellationToken(IMqttModelBindingContext mqttModelBindingContext, IDictionary<ParameterInfo, object?> actionArguments, ParameterInfo actionParameter)
    {
        if (actionParameter.ParameterType != typeof(CancellationToken))
        {
            return false;
        }

        var argumentValue = mqttModelBindingContext.Request.ShutdownToken;
        actionArguments[actionParameter] = argumentValue;

        return true;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindParameter(IMqttModelBindingContext mqttModelBindingContext, IDictionary<ParameterInfo, object?> actionArguments, ParameterInfo actionParameter)
    {
        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttTopicAttribute, IMqttParameterTypeConverter>(
            mqttModelBindingContext,
            actionParameter,
            a => a.TypeConverterType);

        if (!mqttModelBindingContext.TopicArguments.TryGetValue(actionParameter.Name!, out var argumentValueStrings))
        {
            return false;
        }

        if (argumentValueStrings.Length == 0)
        {
            actionArguments[actionParameter] = null;
            return true;
        }

        var actionParameterParameterType = actionParameter.ParameterType;
        if (actionParameterParameterType.IsArray)
        {
            var elementType = actionParameterParameterType.GetElementType()!;
            var array = Array.CreateInstance(elementType, argumentValueStrings.Length);
            for (var i = 0; i < argumentValueStrings.Length; i++)
            {
                if (!mqttModelBindingContext.TryConvertParameter(argumentValueStrings[i], explicitTypeConverter, actionParameter, elementType, out var typedArgumentValue))
                {
                    mqttModelBindingContext.Logger?.LogWarning(
                        "Unable to convert parameter {ParameterName} value to {ParameterType}",
                        actionParameter.Name,
                        elementType
                    );

                    return false;
                }

                array.SetValue(typedArgumentValue, i);
            }

            actionArguments[actionParameter] = array;
            return true;
        }

        if (mqttModelBindingContext.TryConvertParameter(argumentValueStrings.Single(), explicitTypeConverter, actionParameter, actionParameterParameterType, out var argumentValue))
        {
            actionArguments[actionParameter] = argumentValue;
            return true;
        }

        mqttModelBindingContext.Logger?.LogWarning(
            "Unable to convert parameter {ParameterName} value to {ParameterType}",
            actionParameter.Name,
            actionParameterParameterType
        );

        return false;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindCorrelationData(IMqttModelBindingContext mqttModelBindingContext, IDictionary<ParameterInfo, object?> actionArguments, ParameterInfo actionParameter)
    {
        var parameterType = actionParameter.ParameterType;

        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttPayloadAttribute, IMqttPayloadTypeConverter>(
            mqttModelBindingContext,
            actionParameter,
            a => a.TypeConverterType);

        var correlationData = new ArraySegment<byte>(mqttModelBindingContext.Request.CorrelationData ?? Array.Empty<byte>());

        if (explicitTypeConverter != null && explicitTypeConverter.TryConvertPayload(correlationData, parameterType, out var correlationDataResult))
        {
            actionArguments[actionParameter] = correlationDataResult;
            return true;
        }

        if (DefaultTypeConverters.TryConvert(correlationData, parameterType, out correlationDataResult))
        {
            actionArguments[actionParameter] = correlationDataResult;
            return true;
        }

        if (mqttModelBindingContext.CorrelationDataTypeConverter.TryConvertCorrelationData(mqttModelBindingContext.Request.CorrelationData, parameterType, out correlationDataResult))
        {
            actionArguments[actionParameter] = correlationDataResult;
            return true;
        }

        actionArguments[actionParameter] = null!;
        return false;
    }

    // ReSharper disable once UnusedParameter.Local
    [ExcludeFromCodeCoverage]
    private bool TryBindUserProperty(IMqttModelBindingContext mqttModelBindingContext, IDictionary<ParameterInfo, object?> actionArguments, ParameterInfo actionParameter)
    {
        return false;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindService(IMqttModelBindingContext mqttModelBindingContext, IDictionary<ParameterInfo, object?> actionArguments, ParameterInfo actionParameter)
    {
        var parameterType = actionParameter.ParameterType;
        if (parameterType.IsEnum ||
            parameterType.IsPrimitive ||
            parameterType == typeof(string) ||
            Nullable.GetUnderlyingType(parameterType) != null)
        {
            return false;
        }

        var argumentValues = mqttModelBindingContext.Request.ServiceProvider
            .GetServices(parameterType)
            .ToArray();

        if (argumentValues.Length == 0) return false;

        var argumentValue = argumentValues.Last();
        actionArguments[actionParameter] = argumentValue;

        return true;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindPayload(IMqttModelBindingContext mqttModelBindingContext, ParameterInfo? payloadParameter, out object? payload)
    {
        if (payloadParameter == null)
        {
            payload = null;
            return false;
        }

        var parameterType = payloadParameter.ParameterType;

        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttPayloadAttribute, IMqttPayloadTypeConverter>(
            mqttModelBindingContext,
            payloadParameter,
            a => a.TypeConverterType);

        if (explicitTypeConverter != null && explicitTypeConverter.TryConvertPayload(mqttModelBindingContext.Request.Payload, parameterType, out var payloadResult))
        {
            payload = payloadResult;
            return true;
        }

        if (DefaultTypeConverters.TryConvert(mqttModelBindingContext.Request.Payload, parameterType, out payloadResult))
        {
            payload = payloadResult;
            return true;
        }

        if (mqttModelBindingContext.PayloadTypeConverter.TryConvertPayload(mqttModelBindingContext.Request.Payload, parameterType, out payloadResult))
        {
            payload = payloadResult;
            return true;
        }

        payload = null!;
        return false;
    }
}
