using System;
using System.Collections.Generic;
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

namespace Sholo.Mqtt.Routing;

public class RouteProvider : IRouteProvider
{
    public IMqttTopicFilter[] TopicFilters { get; }
    private Endpoint[] Endpoints { get; }

    public Endpoint? GetEndpoint(IMqttRequestContext requestContext)
    {
        var modelBinder = requestContext.ServiceProvider.GetRequiredService<IMqttModelBinder>();

        foreach (var endpoint in Endpoints)
        {
            if (!endpoint.TopicFilter.IsMatch(requestContext, out var topicArguments))
            {
                continue;
            }

            modelBinder.TryPerformModelBinding(endpoint, requestContext, topicArguments);

            if (requestContext.ModelBindingResult is { Success: true })
            {
                return endpoint;
            }
        }

        return null;
    }

    public RouteProvider(IEnumerable<MqttApplicationPart> mqttApplicationParts)
    {
        Endpoints = BuildEndpoints(mqttApplicationParts);
        TopicFilters = Endpoints.Select(x => x.TopicFilter).ToArray();
    }

    private Endpoint[] BuildEndpoints(IEnumerable<MqttApplicationPart> mqttApplicationParts)
    {
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
                .Select(m => CreateEndpoint(ctrl.GetTypeInfo(), m)))
            .Where(x => x != null)
            .Select(x => x!)
            .ToArray();

        return endpoints;
    }

    private Endpoint CreateEndpoint(
        TypeInfo instance,
        MethodInfo action)
    {
        if (instance == null)
        {
            throw new ArgumentException("An instance is required for controller-based routes");
        }

        var mqttTopicFilter = MqttTopicFilterBuilder
            .FromActionAttributes(instance, action)
            .Build();

        var controllerName = instance.Name.EndsWith("Controller", StringComparison.Ordinal)
            ? instance.Name[..^"Controller".Length]
            : instance.Name;

        var requestDelegate = CreateRequestDelegate(
            instance,
            action,
            controllerName,
            mqttTopicFilter
        );

        return new Endpoint(
            instance,
            action,
            mqttTopicFilter,
            requestDelegate
        );
    }

    private MqttRequestDelegate CreateRequestDelegate(
        Type controllerType,
        MethodInfo action,
        string controllerName,
        IMqttTopicFilter topicFilter)
    {
        return async requestContext =>
        {
            if (requestContext.ModelBindingResult is not { Success: true })
            {
                throw new InvalidOperationException("Model binding did not complete successfully");
            }

            object? controllerInstance = null;
            var controllerActivator = requestContext.ServiceProvider.GetRequiredService<IMqttControllerActivator>();

            try
            {
                controllerInstance = controllerActivator.Create(requestContext, controllerType);
                if (controllerInstance is MqttControllerBase controllerBase)
                {
                    controllerBase.Request = requestContext;
                }

                var logger = requestContext.ServiceProvider.GetService<ILogger<RouteProvider>>();
                var requestDelegate = requestContext.GetRequestDelegate(controllerInstance);

                using var scope = logger?.BeginScope(new Dictionary<string, string>
                {
                    ["TopicPattern"] = topicFilter.TopicPattern,
                    ["Controller"] = controllerName,
                    ["ActionName"] = action.Name
                });

                return await requestDelegate.Invoke(requestContext);
            }
            finally
            {
                if (controllerInstance != null)
                {
                    await controllerActivator.ReleaseAsync(requestContext, controllerInstance);
                }
            }
        };
    }
}
