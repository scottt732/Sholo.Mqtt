namespace Sholo.Mqtt.Routing;

/*
public class AnonymousRouteProvider : BaseRouteProvider
{
    public AnonymousRouteProvider(Endpoint endpoint)
    {
        Endpoints = new[] { endpoint };
    }

    public override Endpoint? GetEndpoint(IMqttRequestContext requestContext)
    {
        throw new NotImplementedException();
    }

    private Endpoint? CreateEndpoint(
        TypeInfo? instance,
        MethodInfo action)
    {
        if (!TryCreateTopicFilterFromAttributes(instance, action, out var topicFilter))
        {
            return null;
        }

        var requestDelegate = CreateAnonymousRequestDelegate(
            instance,
            action,
            topicFilter
        );

        return new Endpoint(
            instance,
            action,
            topicFilter,
            requestDelegate
        );
    }

    private MqttRequestDelegate CreateAnonymousRequestDelegate(
        Type? instance,
        MethodInfo action,
        IMqttTopicFilter topicFilter)
    {
        return async requestContext =>
        {
            if (requestContext.ModelBindingResult is not { Success: true })
            {
                throw new InvalidOperationException("Model binding did not complete successfully");
            }

            var logger = requestContext.ServiceProvider.GetService<ILogger<RouteProvider>>();
            var requestDelegate = requestContext.GetRequestDelegate(instance);

            using var scope = logger?.BeginScope(new Dictionary<string, string>
            {
                ["TopicPattern"] = topicFilter.TopicPattern,
                ["ActionName"] = action.Name
            });

            return await requestDelegate.Invoke(requestContext);
        };
    }
}
*/
