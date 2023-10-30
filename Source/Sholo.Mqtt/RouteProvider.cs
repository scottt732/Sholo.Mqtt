using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.Controllers;
using Sholo.Mqtt.Internal;
using Sholo.Mqtt.Topics.PatternFilter;
using Sholo.Mqtt.Topics.PatternFilterBuilder;
using Sholo.Mqtt.TypeConverters;
using Sholo.Mqtt.TypeConverters.Parameter;
using Sholo.Mqtt.TypeConverters.Payload;

namespace Sholo.Mqtt;

public class RouteProvider : IRouteProvider
{
    public Endpoint[] Endpoints { get; }

    private IControllerActivator ControllerActivator { get; }

    public Endpoint GetEndpoint(MqttRequestContext context)
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
            .ToArray();

        Endpoints = endpoints;
    }

    private Endpoint GetEndpoint(
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

        var topicPatternFilter = GetTopicPatternFilter(
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
            topicPatternFilter);

        return new Endpoint(
            action,
            topicPatternFilter,
            requestDelegate);
    }

    private MqttRequestDelegate CreateRequestDelegate(
        Type controllerType,
        MethodInfo action,
        string topicName,
        string controllerName,
        IMqttTopicPatternFilter topicPatternFilter
    )
    {
        return async requestContext =>
        {
            var logger = requestContext.ServiceProvider.GetService<ILogger<RouteProvider>>();

            var actionParameters = action.GetParameters();

            var parametersBindingContext = new ParametersBindingContext(
                action,
                topicName,
                topicPatternFilter,
                requestContext,
                logger,
                null
            );

            if (!TryBindActionParameters(parametersBindingContext, out var actionArguments))
            {
                return false;
            }

            if (actionArguments.Count != actionParameters.Length)
            {
                var unmatchedParameters = string.Join(", ", actionArguments.Keys.Except(actionParameters).Select(x => x.Name));
                logger?.LogDebug("Failed to bind the following parameters: {UnmatchedParameters}", unmatchedParameters);
                return false;
            }

            if (!parametersBindingContext.PayloadState.IsValid)
            {
            }

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

            var resultTask = (Task<bool>)action.Invoke(controllerInstance, actionArguments.Values.ToArray());

            var result = await resultTask!;

            logger?.LogDebug(
                "Executed {TopicName} ({Controller}.{Action}) in {Duration:F0}ms",
                topicName,
                controllerName,
                action.Name,
                stopwatch.ElapsedMilliseconds);

            await ControllerActivator.ReleaseAsync(requestContext, controllerInstance);

            return result;
        };
    }

    private bool TryBindActionParameters(ParametersBindingContext parametersBindingContext, out IDictionary<ParameterInfo, object> actionArguments)
    {
        actionArguments = new Dictionary<ParameterInfo, object>();

        if (!parametersBindingContext.TopicPatternFilter.IsMatch(parametersBindingContext.Request.Topic, out var readWriteTopicArguments))
        {
            return false;
        }

        var topicArguments = new ReadOnlyDictionary<string, string>(readWriteTopicArguments);

        ParameterInfo unboundParameter = null;
        var actionParameters = parametersBindingContext.Action.GetParameters();

        foreach (var actionParameter in actionParameters)
        {
            var parameterBindingContext = new ParameterBindingContext(
                parametersBindingContext,
                topicArguments,
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
                return false;
            }

            // Need to hold the slot. We'll supply the value below.
            actionArguments.Add(actionParameter, null);
            unboundParameter = actionParameter;
        }

        if (unboundParameter == null)
        {
            return true;
        }

        var payloadBindingContext = new PayloadBindingContext(
            parametersBindingContext,
            topicArguments,
            actionArguments,
            unboundParameter);

        if (!TryBindAndValidateActionPayload(payloadBindingContext, out var isValid))
        {
            return false;
        }

        if (!isValid)
        {
            // var body = Encoding.ASCII.GetString(payloadBindingContext.Request.Payload);

            payloadBindingContext.Logger?.LogWarning(
                "Request validation failed: {ValidationErrors}",
                string.Join(", ", payloadBindingContext.PayloadState.Results.Select(x => $"{x.ErrorMessage}")));
        }

        return true;
    }

    private bool TryBindAndValidateActionPayload(PayloadBindingContext payloadBindingContext, out bool isValid)
    {
        if (!TryBindPayload(payloadBindingContext))
        {
            isValid = false;
            return false;
        }

        isValid = TryValidatePayload(payloadBindingContext);

        return true;
    }

    private bool TryValidatePayload(PayloadBindingContext payloadBindingContext)
    {
        var validationContext = new ValidationContext(payloadBindingContext.Payload);

        var validationResults = new List<ValidationResult>();

        var success = Validator.TryValidateObject(
            payloadBindingContext.Payload,
            validationContext,
            validationResults,
            validateAllProperties: true);

        foreach (var validationResult in validationResults)
        {
            payloadBindingContext.PayloadState.Results.Add(validationResult);
        }

        return success;
    }

    private bool TryBindCancellationToken(ParameterBindingContext parameterBindingContext)
    {
        if (parameterBindingContext.ActionParameter.ParameterType != typeof(CancellationToken))
        {
            return false;
        }

        var argumentValue = parameterBindingContext.Request.ShutdownToken;
        parameterBindingContext.Value = argumentValue;

        return true;
    }

    private bool TryBindParameter(ParameterBindingContext parameterBindingContext)
    {
        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttTopicAttribute, IMqttParameterTypeConverter>(
            parameterBindingContext,
            parameterBindingContext.ActionParameter,
            a => a.TypeConverterType);

        if (!parameterBindingContext.TopicArguments.TryGetValue(parameterBindingContext.ActionParameter.Name!, out var argumentValueString))
        {
            return false;
        }

        if (argumentValueString == null)
        {
            parameterBindingContext.Value = null;
            return true;
        }

        if (parameterBindingContext.TryConvert(argumentValueString, explicitTypeConverter, parameterBindingContext.ActionParameter.ParameterType, out var argumentValue))
        {
            parameterBindingContext.Value = argumentValue;
            return true;
        }

        parameterBindingContext.Logger?.LogWarning(
            "Unable to convert parameter {ParameterName} value to {ParameterType}",
            parameterBindingContext.ActionParameter.Name,
            parameterBindingContext.ActionParameter.ParameterType
        );

        return false;
    }

    // ReSharper disable UnusedParameter.Local

#pragma warning disable IDE0060 // Remove unused parameter - TODO: WiP
    private bool TryBindCorrelationData(ParameterBindingContext parameterBindingContext)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        // parameterBindingContext.Value = null;
        return false;
    }

#pragma warning disable IDE0060 // Remove unused parameter - TODO: WiP
    private bool TryBindUserProperty(ParameterBindingContext parameterBindingContext)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        // parameterBindingContext.Value = null;
        return false;
    }

    // ReSharper restore UnusedParameter.Local

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

        if (!argumentValues.Any()) return false;

        var argumentValue = argumentValues.Last();

        parameterBindingContext.Value = argumentValue;

        return true;
    }

    private bool TryBindPayload(PayloadBindingContext payloadBindingContext)
    {
        var parameterType = payloadBindingContext.PayloadParameter.ParameterType;

        var explicitTypeConverter = CreateRequestedTypeConverter<FromMqttPayloadAttribute, IMqttRequestPayloadTypeConverter>(
            payloadBindingContext,
            payloadBindingContext.PayloadParameter,
            a => a.TypeConverterType);

        if (explicitTypeConverter != null && explicitTypeConverter.TryConvertPayload(payloadBindingContext.Request.Payload, parameterType, out var payloadResult))
        {
            payloadBindingContext.Payload = payloadResult;
            return true;
        }

        if (DefaultTypeConverters.TryConvert(payloadBindingContext.Request.Payload, parameterType, out payloadResult))
        {
            payloadBindingContext.Payload = payloadResult;
            return true;
        }

        if (payloadBindingContext.PayloadTypeConverter.TryConvertPayload(payloadBindingContext.Request.Payload, parameterType, out payloadResult))
        {
            payloadBindingContext.Payload = payloadResult;
            return true;
        }

        payloadBindingContext.Payload = default;
        return false;
    }

    private TTypeConverter CreateRequestedTypeConverter<TAttribute, TTypeConverter>(
        ParametersBindingContext parametersBindingContext,
        ParameterInfo actionParameter,
        Func<TAttribute, Type> typeConverterTypeSelector)
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

    private IMqttTopicPatternFilter GetTopicPatternFilter(
        TopicPrefixAttribute topicPrefixAttribute,
        TopicAttribute topicAttribute,
        NoLocalAttribute noLocalAttribute,
        QualityOfServiceAttribute qualityOfServiceAttribute,
        RetainAsPublishedAttribute retainAsPublishedAttribute,
        RetainHandlingAttribute retainHandlingAttribute)
    {
        var topicPrefix = topicPrefixAttribute?.TopicPrefix?.TrimEnd('/');
        var topicPattern = topicAttribute?.TopicPattern.TrimStart('/');
        var topic = !string.IsNullOrEmpty(topicPrefix)
            ? $"{topicPrefix}/{topicPattern}"
            : topicPattern;

        var noLocal = noLocalAttribute?.NoLocal;
        var qualityOfServiceLevel = qualityOfServiceAttribute?.QualityOfServiceLevel;
        var retainAsPublished = retainAsPublishedAttribute?.RetainAsPublished;
        var retainHandling = retainHandlingAttribute?.RetainHandling;

        var mqttTopicFilterBuilder = new MqttTopicPatternFilterBuilder();

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

        mqttTopicFilterBuilder.WithTopicPattern(topic);

        var mqttTopicPatternFilter = mqttTopicFilterBuilder.Build();

        return mqttTopicPatternFilter;
    }
}
