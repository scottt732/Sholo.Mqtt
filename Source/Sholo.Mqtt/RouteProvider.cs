using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.Controllers;
using Sholo.Mqtt.Internal;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.FilterBuilder;
using Sholo.Mqtt.TypeConverters;
using Sholo.Mqtt.TypeConverters.Parameter;
using Sholo.Mqtt.TypeConverters.Payload;
using Sholo.Mqtt.Utilities;

namespace Sholo.Mqtt;

public class RouteProvider : IRouteProvider
{
    public Endpoint[] Endpoints { get; }

    private IControllerActivator ControllerActivator { get; }

    public Endpoint? GetEndpoint(IMqttRequestContext context)
    {
        return Endpoints.FirstOrDefault(endpoint => endpoint.IsMatch(context));
    }

    public RouteProvider(
        IControllerActivator controllerActivator,
        IEnumerable<MqttApplicationPart> mqttApplicationParts)
    {
        ControllerActivator = controllerActivator;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(asm => asm.GetCustomAttributes<MqttApplicationPartAttribute>().Any())
            .Union(mqttApplicationParts.Select(ap => ap.Assembly))
            .Distinct();

        var controllers = assemblies
            .SelectMany(asm => asm
                .GetExportedTypes()
                .Where(c => c.IsClass)
                .Where(c => c.IsPublic)
                .Where(c => c.GetCustomAttributes<MqttControllerAttribute>().Any()));

        var endpoints = controllers
            .SelectMany(ctrl => ctrl
                .GetMethods()
                .Where(m => m.IsPublic)
                .Where(m => !m.IsGenericMethod)
                .Where(m => m.ReturnType == typeof(bool) || m.ReturnType == typeof(Task<bool>))
                .Select(m => GetEndpoint(ctrl, m)))
            .Where(x => x != null)
            .Select(x => x!)
            .ToArray();

        Endpoints = endpoints;
    }

    [ExcludeFromCodeCoverage]
    private Endpoint? GetEndpoint(
        Type controller,
        MethodInfo action)
    {
        var topicPrefixAttribute = controller.GetCustomAttribute<TopicPrefixAttribute>();

        var topicAttribute = action.GetCustomAttribute<TopicAttribute>() ??
                             controller.GetCustomAttribute<TopicAttribute>();

        if (topicAttribute == null)
        {
            return null;
        }

        var noLocalAttribute = action.GetCustomAttribute<NoLocalAttribute>() ??
                               controller.GetCustomAttribute<NoLocalAttribute>();

        var qualityOfServiceAttribute = action.GetCustomAttribute<QualityOfServiceAttribute>() ??
                                        controller.GetCustomAttribute<QualityOfServiceAttribute>();

        var retainAsPublishedAttribute = action.GetCustomAttribute<RetainAsPublishedAttribute>() ??
                                         controller.GetCustomAttribute<RetainAsPublishedAttribute>();

        var retainHandlingAttribute = action.GetCustomAttribute<RetainHandlingAttribute>() ??
                                      controller.GetCustomAttribute<RetainHandlingAttribute>();

        var topicFilter = GetTopicFilter(
            topicPrefixAttribute,
            topicAttribute,
            noLocalAttribute,
            qualityOfServiceAttribute,
            retainAsPublishedAttribute,
            retainHandlingAttribute);

        var controllerName = controller.Name.EndsWith("Controller", StringComparison.Ordinal)
            ? controller.Name[..^"Controller".Length]
            : controller.Name;

        var topicName = topicAttribute.Name ?? action.Name;

        var requestDelegate = CreateRequestDelegate(
            controller,
            action,
            topicName,
            controllerName,
            topicFilter);

        return new Endpoint(
            action,
            topicFilter,
            requestDelegate);
    }

    [ExcludeFromCodeCoverage]
    private MqttRequestDelegate CreateRequestDelegate(
        Type controllerType,
        MethodInfo action,
        string topicName,
        string controllerName,
        IMqttTopicFilter topicPatternFilter
    )
    {
        return async requestContext =>
        {
            // See if the request message's topic matches the pattern & extract arguments
            if (!topicPatternFilter.IsMatch(requestContext, out var topicArguments))
            {
                return false;
            }

            var logger = requestContext.ServiceProvider.GetService<ILogger<RouteProvider>>();

            var actionParameters = action.GetParameters();
            var parametersBindingContext = new ParametersBindingContext(
                action,
                topicName,
                requestContext,
                topicArguments!,
                logger
            );

            // Attempt to bind the topic arguments, services, etc. to the action/method parameters
            if (!TryBindActionParameters(parametersBindingContext, out var actionArguments, out var payloadParameter))
            {
                return false;
            }

            // Handle the case where we couldn't match 100% of the action arguments to parameters
            if (actionArguments.Count != actionParameters.Length)
            {
                var unmatchedParameters = string.Join(", ", actionArguments.Keys.Except(actionParameters).Select(x => x.Name));

                logger?.LogDebug(
                    "Evaluating candidate handler {TopicName} ({Controller}.{Action}): Failed to bind the following parameters: {UnmatchedParameters}",
                    topicName,
                    controllerName,
                    action.Name,
                    unmatchedParameters
                );
                return false;
            }

            // Handle the case where the bound payload is invalid
            var payloadBindingContext = new PayloadBindingContext(
                parametersBindingContext,
                actionArguments,
                payloadParameter!
            );

            if (!TryBindAndValidateActionPayload(payloadBindingContext, out var payload, out var validationResults))
            {
                logger?.LogWarning(
                    "Evaluating candidate handler {TopicName} ({Controller}.{Action}): The message payload failed validation:{NewLine}{ValidationErrors}",
                    topicName,
                    controllerName,
                    action.Name,
                    Environment.NewLine,
                    string.Join(Environment.NewLine, validationResults!.Select(x => $"{x.ErrorMessage} ({string.Join(", ", x.MemberNames)})"))
                );
                return false;
            }

            actionArguments[payloadParameter!] = payload;

            logger?.LogDebug(
                "Executing {TopicName} ({Controller}.{Action})",
                topicName,
                controllerName,
                action.Name);

            var stopwatch = Stopwatch.StartNew();

            var controllerInstance = ControllerActivator.Create(requestContext, controllerType);

            if (controllerInstance is MqttControllerBase controllerBase)
            {
                controllerBase.Request = requestContext;
            }

            var result = false;
            try
            {
                var resultTask = (Task<bool>)action.Invoke(controllerInstance, actionArguments.Values.ToArray())!;
                result = await resultTask;

                logger?.LogDebug(
                    "Executed {TopicName} ({Controller}.{Action}) in {Duration:F0}ms",
                    topicName,
                    controllerName,
                    action.Name,
                    stopwatch.ElapsedMilliseconds
                );
            }
            catch (Exception exc)
            {
                logger?.LogError(
                    exc,
                    "Error executing {TopicName} ({Controller}.{Action}): {Message}",
                    topicName,
                    controllerName,
                    action.Name,
                    exc.Message
                );
            }
            finally
            {
                await ControllerActivator.ReleaseAsync(requestContext, controllerInstance);
            }

            return result;
        };
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindActionParameters(IParametersBindingContext parametersBindingContext, out IDictionary<ParameterInfo, object?> actionArguments, out ParameterInfo? payloadParameter)
    {
        actionArguments = new Dictionary<ParameterInfo, object?>();

        ParameterInfo? unboundParameter = null;
        var actionParameters = parametersBindingContext.Action.GetParameters();

        foreach (var actionParameter in actionParameters)
        {
            var parameterBindingContext = new ParameterBindingContext(
                parametersBindingContext,
                actionArguments,
                actionParameter
            );

            if (TryBindCancellationToken(parameterBindingContext) ||
                TryBindParameter(parameterBindingContext) ||
                TryBindCorrelationData(parameterBindingContext) ||
                TryBindUserProperty(parameterBindingContext) ||
                TryBindService(parameterBindingContext))
            {
                continue;
            }

            if (unboundParameter != null)
            {
                // There can only be 1 unmatched parameter (the one that might be the payload)
                payloadParameter = null;
                return false;
            }

            // Need to hold the slot. We'll supply the value below.
            actionArguments.Add(actionParameter, null);
            unboundParameter = actionParameter;
        }

        if (unboundParameter == null)
        {
            parametersBindingContext.Logger?.LogWarning("No remaining parameters available for payload binding");
            payloadParameter = null;
            return false;
        }

        payloadParameter = unboundParameter;
        return true;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindAndValidateActionPayload(IPayloadBindingContext payloadBindingContext, out object? payload, out ValidationResult[]? validationResults)
    {
        if (TryBindPayload(payloadBindingContext, out payload) && ValidationHelper.IsValid(payload!, out validationResults))
        {
            return true;
        }

        payload = null;
        validationResults = null;
        return false;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindCancellationToken(IParameterBindingContext parameterBindingContext)
    {
        if (parameterBindingContext.ActionParameter.ParameterType != typeof(CancellationToken))
        {
            return false;
        }

        var argumentValue = parameterBindingContext.Request.ShutdownToken;
        parameterBindingContext.Value = argumentValue;

        return true;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindParameter(IParameterBindingContext parameterBindingContext)
    {
        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttTopicAttribute, IMqttParameterTypeConverter>(
            parameterBindingContext,
            parameterBindingContext.ActionParameter,
            a => a.TypeConverterType);

        if (!parameterBindingContext.TopicArguments.TryGetValue(parameterBindingContext.ActionParameter.Name!, out var argumentValueStrings))
        {
            return false;
        }

        if (argumentValueStrings.Length == 0)
        {
            parameterBindingContext.Value = null;
            return true;
        }

        var actionParameterParameterType = parameterBindingContext.ActionParameter.ParameterType;
        if (actionParameterParameterType.IsArray)
        {
            var elementType = actionParameterParameterType.GetElementType()!;
            var array = Array.CreateInstance(elementType, argumentValueStrings.Length);
            for (var i = 0; i < argumentValueStrings.Length; i++)
            {
                if (!parameterBindingContext.TryConvert(argumentValueStrings[i], explicitTypeConverter, elementType, out var typedArgumentValue))
                {
                    parameterBindingContext.Logger?.LogWarning(
                        "Unable to convert parameter {ParameterName} value to {ParameterType}",
                        parameterBindingContext.ActionParameter.Name,
                        elementType
                    );

                    return false;
                }

                array.SetValue(typedArgumentValue, i);
            }

            parameterBindingContext.Value = array;
            return true;
        }

        if (parameterBindingContext.TryConvert(argumentValueStrings.Single(), explicitTypeConverter, actionParameterParameterType, out var argumentValue))
        {
            parameterBindingContext.Value = argumentValue;
            return true;
        }

        parameterBindingContext.Logger?.LogWarning(
            "Unable to convert parameter {ParameterName} value to {ParameterType}",
            parameterBindingContext.ActionParameter.Name,
            actionParameterParameterType
        );

        return false;
    }

    // ReSharper disable once UnusedParameter.Local
    [ExcludeFromCodeCoverage]
    private bool TryBindCorrelationData(ParameterBindingContext parameterBindingContext)
    {
        return false;
    }

    // ReSharper disable once UnusedParameter.Local
    [ExcludeFromCodeCoverage]
    private bool TryBindUserProperty(ParameterBindingContext parameterBindingContext)
    {
        return false;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindService(ParameterBindingContext parameterBindingContext)
    {
        var parameterType = parameterBindingContext.ActionParameter.ParameterType;
        if (parameterType.IsEnum ||
            parameterType.IsPrimitive ||
            parameterType == typeof(string) ||
            Nullable.GetUnderlyingType(parameterType) != null)
        {
            return false;
        }

        var argumentValues = parameterBindingContext.Request.ServiceProvider
                .GetServices(parameterType)
                .ToArray();

        if (argumentValues.Length == 0) return false;

        var argumentValue = argumentValues.Last();

        parameterBindingContext.Value = argumentValue;

        return true;
    }

    [ExcludeFromCodeCoverage]
    private bool TryBindPayload(IPayloadBindingContext payloadBindingContext, out object? payload)
    {
        var parameterType = payloadBindingContext.PayloadParameter.ParameterType;

        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttPayloadAttribute, IMqttRequestPayloadTypeConverter>(
            payloadBindingContext,
            payloadBindingContext.PayloadParameter,
            a => a.TypeConverterType);

        if (explicitTypeConverter != null && explicitTypeConverter.TryConvertPayload(payloadBindingContext.Request.Payload, parameterType, out var payloadResult))
        {
            payload = payloadResult;
            return true;
        }

        if (DefaultTypeConverters.TryConvert(payloadBindingContext.Request.Payload, parameterType, out payloadResult))
        {
            payload = payloadResult;
            return true;
        }

        if (payloadBindingContext.PayloadTypeConverter.TryConvertPayload(payloadBindingContext.Request.Payload, parameterType, out payloadResult))
        {
            payload = payloadResult;
            return true;
        }

        payload = null!;
        return false;
    }

    [ExcludeFromCodeCoverage]
    private TTypeConverter? CreateRequestedTypeConverter<TAttribute, TTypeConverter>(
        IParametersBindingContext parametersBindingContext,
        ParameterInfo actionParameter,
        Func<TAttribute, Type?> typeConverterTypeSelector)
        where TAttribute : Attribute
        where TTypeConverter : class
    {
        var customAttribute = actionParameter.GetCustomAttribute<TAttribute>();
        if (customAttribute == null) return null;

        var explicitTypeConverterType = typeConverterTypeSelector.Invoke(customAttribute);
        if (explicitTypeConverterType == null) return null;

        var serviceProvider = parametersBindingContext.Request.ServiceProvider;

        var typeConverterInstance = serviceProvider.GetService(explicitTypeConverterType) ??
                                    Activator.CreateInstance(explicitTypeConverterType);

        if (typeConverterInstance is not TTypeConverter typeConverter)
        {
            throw new InvalidOperationException($"Unable to resolve {explicitTypeConverterType.Name}");
        }

        return typeConverter;
    }

    [ExcludeFromCodeCoverage]
    private IMqttTopicFilter GetTopicFilter(
        TopicPrefixAttribute? topicPrefixAttribute,
        TopicAttribute topicAttribute,
        NoLocalAttribute? noLocalAttribute,
        QualityOfServiceAttribute? qualityOfServiceAttribute,
        RetainAsPublishedAttribute? retainAsPublishedAttribute,
        RetainHandlingAttribute? retainHandlingAttribute)
    {
        var topicPrefix = topicPrefixAttribute?.TopicPrefix.TrimEnd('/');
        var topicPattern = topicAttribute.TopicPattern.TrimStart('/');
        var effectiveTopicPattern = !string.IsNullOrEmpty(topicPrefix)
            ? $"{topicPrefix}/{topicPattern}"
            : topicPattern;

        var noLocal = noLocalAttribute?.NoLocal;
        var qualityOfServiceLevel = qualityOfServiceAttribute?.QualityOfServiceLevel;
        var retainAsPublished = retainAsPublishedAttribute?.RetainAsPublished;
        var retainHandling = retainHandlingAttribute?.RetainHandling;

        var mqttTopicFilterBuilder = new MqttTopicFilterBuilder();

        if (noLocal.HasValue)
        {
            mqttTopicFilterBuilder.WithNoLocal(noLocal.Value);
        }

        if (qualityOfServiceLevel.HasValue)
        {
            mqttTopicFilterBuilder.WithQualityOfServiceLevel(qualityOfServiceLevel.Value);
        }

        if (retainAsPublished.HasValue)
        {
            mqttTopicFilterBuilder.WithRetainAsPublished(retainAsPublished.Value);
        }

        if (retainHandling.HasValue)
        {
            mqttTopicFilterBuilder.WithRetainHandling(retainHandling.Value);
        }

        mqttTopicFilterBuilder.WithTopicPattern(effectiveTopicPattern);

        var mqttTopicFilter = mqttTopicFilterBuilder.Build();

        return mqttTopicFilter;
    }
}
