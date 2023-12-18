using Sholo.Mqtt.Application.Builder;
using Sholo.Mqtt.Middleware;

namespace Sholo.Mqtt.Routing;

[PublicAPI]
public static class MqttApplicationBuilderExtensions
{
    public static IMqttApplicationBuilder UseRouting(this IMqttApplicationBuilder mqttApplicationBuilder)
    {
        return mqttApplicationBuilder.UseMiddleware<RoutingMiddleware>();
    }
}

/*
[PublicAPI]
public static class MqttApplicationBuilderExtensions
{
    public static IMqttApplicationBuilder UseRouting(this IMqttApplicationBuilder mqttApplicationBuilder)
    {
        return mqttApplicationBuilder.UseMiddleware<RoutingMiddleware>();
    }

    public static IMqttApplicationBuilder Use(this IMqttApplicationBuilder mqttApplicationBuilder, Action<IMqttTopicFilterBuilder> topicFilterBuilderConfig, MqttRequestDelegate requestDelegate)
    {
        var mqttTopicFilterBuilder = new MqttTopicFilterBuilder();
        topicFilterBuilderConfig.Invoke(mqttTopicFilterBuilder);
        var mqttTopicFilter = mqttTopicFilterBuilder.Build();
        // var endpoint = new Endpoint(null, requestDelegate.Method, mqttTopicFilter, ctx => ctx.ModelBindingResult.Invoke())

        /*
        new AnonymousRouteProvider()
        var endpoint = new Endpoint(null, requestDelegate.Method, mqttTopicFilter, )
        // mqttApplicationBuilder.UseMiddleware(new Anon);
        // mqttApplicationBuilder.UseMiddleware();
        * /
    }
}

public class AnonymousRouteProvider : IRouteProvider
{
    public IMqttTopicFilter[] TopicFilters { get; }
    public Endpoint? GetEndpoint(IMqttRequestContext requestContext)
    {
        throw new NotImplementedException();
    }

    public AnonymousRouteProvider(IMqttTopicFilter topicFilter)
    {
        TopicFilters = new[] { topicFilter };
    }
}
*/
