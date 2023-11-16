using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.Controllers;
using Sholo.Mqtt.Internal;
using Sholo.Mqtt.ModelBinding;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.FilterBuilder;

namespace Sholo.Mqtt;

public class RouteProvider : IRouteProvider
{
    public Endpoint[] Endpoints { get; }

    private IMqttModelBinder ModelBinder { get; }
    private IControllerActivator ControllerActivator { get; }

    public Endpoint? GetEndpoint(IMqttRequestContext context)
    {
        return Endpoints.FirstOrDefault(endpoint => endpoint.IsMatch(context));
    }

    public RouteProvider(
        IMqttModelBinder modelBinder,
        IControllerActivator controllerActivator,
        IEnumerable<MqttApplicationPart> mqttApplicationParts)
    {
        ModelBinder = modelBinder;
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

        var topicFilter = CreateTopicFilter(
            topicPrefixAttribute,
            topicAttribute,
            noLocalAttribute,
            qualityOfServiceAttribute,
            retainAsPublishedAttribute,
            retainHandlingAttribute);

        var controllerName = controller.Name.EndsWith("Controller", StringComparison.Ordinal)
            ? controller.Name[..^"Controller".Length]
            : controller.Name;

        var requestDelegate = CreateControllerRequestDelegate(
            controller,
            action,
            controllerName,
            topicFilter
        );

        return new Endpoint(
            action,
            topicFilter,
            requestDelegate);
    }

    [ExcludeFromCodeCoverage]
    private MqttRequestDelegate CreateAnonymousRequestDelegate(
        MethodInfo action,
        IMqttTopicFilter topicFilter)
    {
        return async requestContext =>
        {
            if (!ModelBinder.TryPerformModelBinding(requestContext, topicFilter, action, out var actionArguments))
            {
                return false;
            }

            var arguments = actionArguments.Values.ToArray();
            var logger = requestContext.ServiceProvider.GetService<ILogger<RouteProvider>>();

            Func<Task<bool>> actionRunner;
            if (action.ReturnType == typeof(Task<bool>))
            {
                actionRunner = () => (Task<bool>)action.Invoke(null, arguments)!;
            }
            else if (action.ReturnType == typeof(bool))
            {
                actionRunner = () => Task.FromResult((bool)action.Invoke(null, arguments)!);
            }
            else
            {
                throw new InvalidOperationException("Expecting either a Task<bool> or a bool return type");
            }

            using var scope = logger?.BeginScope(new Dictionary<string, string>
            {
                ["TopicPattern"] = topicFilter.TopicPattern
            });

            return await actionRunner.Invoke();
        };
    }

    [ExcludeFromCodeCoverage]
    private MqttRequestDelegate CreateControllerRequestDelegate(
        Type controllerType,
        MethodInfo action,
        string controllerName,
        IMqttTopicFilter topicFilter)
    {
        return async requestContext =>
        {
            if (!ModelBinder.TryPerformModelBinding(requestContext, topicFilter, action, out var actionArguments))
            {
                return false;
            }

            var controllerInstance = ControllerActivator.Create(requestContext, controllerType);
            if (controllerInstance is MqttControllerBase controllerBase)
            {
                controllerBase.Request = requestContext;
            }

            var arguments = actionArguments.Values.ToArray();
            var logger = requestContext.ServiceProvider.GetService<ILogger<RouteProvider>>();

            Func<Task<bool>> actionRunner;
            if (action.ReturnType == typeof(Task<bool>))
            {
                actionRunner = () => (Task<bool>)action.Invoke(controllerInstance, arguments)!;
            }
            else if (action.ReturnType == typeof(bool))
            {
                actionRunner = () => Task.FromResult((bool)action.Invoke(controllerInstance, arguments)!);
            }
            else
            {
                throw new InvalidOperationException("Expecting either a Task<bool> or a bool return type");
            }

            try
            {
                using var scope = logger?.BeginScope(new Dictionary<string, string>
                {
                    ["TopicPattern"] = topicFilter.TopicPattern,
                    ["Controller"] = controllerName,
                    ["ActionName"] = action.Name
                });
                return await actionRunner.Invoke();
            }
            finally
            {
                await ControllerActivator.ReleaseAsync(requestContext, controllerInstance);
            }
        };
    }

    [ExcludeFromCodeCoverage]
    private IMqttTopicFilter CreateTopicFilter(
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
