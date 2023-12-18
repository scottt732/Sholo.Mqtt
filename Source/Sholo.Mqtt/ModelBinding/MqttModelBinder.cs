using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Sholo.Mqtt.ModelBinding.BindingProviders;

namespace Sholo.Mqtt.ModelBinding;

public class MqttModelBinder : IMqttModelBinder
{
    private IMqttParameterBinder[] ParameterBinders { get; }

    public MqttModelBinder(IMqttParameterBinder[] parameterBinders)
    {
        ParameterBinders = parameterBinders;
    }

    public void TryPerformModelBinding(IMqttModelBindingContext modelBindingContext, IMqttRequestContext requestContext, IReadOnlyDictionary<string, StringValues> topicArguments)
    {
        var parameters = modelBindingContext.Action
            .GetParameters()
            .ToDictionary(
                x => x,
                x => new ParameterState(modelBindingContext, x)
            );

        var allParametersSet = true;
        foreach (var (_, parameterState) in parameters)
        {
            if (!TryBind(modelBindingContext, requestContext, topicArguments, parameterState))
            {
                allParametersSet = false;
            }
        }

        if (allParametersSet)
        {
            foreach (var (_, parameterState) in parameters)
            {
                if (parameterState is { IsModelSet: true, ValidationStatus: not ParameterValidationResult.ValidationSuppressed })
                {
                    parameterState.TryValidate();
                }
            }
        }

        requestContext.ModelBindingResult = new MqttModelBindingResult(modelBindingContext, parameters);
    }

    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState
    )
    {
        foreach (var parameterBinder in ParameterBinders)
        {
            if (parameterBinder.TryBind(modelBindingContext, requestContext, topicArguments, parameterState, out var parameterBindingResult))
            {
                parameterState.SetBindingSuccess(parameterBindingResult.BindingSource, parameterBindingResult.Value, parameterBindingResult.BypassValidation);
                return true;
            }
        }

        parameterState.SetBindingFailure();
        return false;
    }

    /*
    public bool TryBindParameters(IMqttUserPropertiesTypeConverter? explicitParameterTypeConverter, ParameterInfo actionParameter, Type targetType, out object? result)
    {
        if (input == null)
        {
            if (actionParameter.ParameterType.IsClass)
            {
                result = default;
                return true;
            }

            if (actionParameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(actionParameter.ParameterType) != null)
            {
                result = null;
                return true;
            }

            result = null;
            return false;
        }

        if (explicitParameterTypeConverter != null)
        {
            if (explicitParameterTypeConverter.TryConvertUserProperties(input, targetType, out result))
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException(
                    $"The converter {explicitParameterTypeConverter.GetType().Name} cannot convert parameters of type {actionParameter.ParameterType.Name}");
            }
        }

        foreach (var converter in ParameterTypeConverters)
        {
            if (converter.TryConvertUserProperties(input, targetType, out result))
            {
                return true;
            }
        }

        if (DefaultTypeConverter.TryConvert(input, actionParameter.ParameterType, out result))
        {
            return true;
        }

        result = default;
        return false;
    }
    */

    /*

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
        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttTopicAttribute, IMqttUserPropertiesTypeConverter>(
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
                if (!mqttModelBindingContext.TryConvertUserProperties(argumentValueStrings[i], explicitTypeConverter, actionParameter, elementType, out var typedArgumentValue))
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

        if (mqttModelBindingContext.TryConvertUserProperties(argumentValueStrings.Single(), explicitTypeConverter, actionParameter, actionParameterParameterType, out var argumentValue))
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
    */
}
