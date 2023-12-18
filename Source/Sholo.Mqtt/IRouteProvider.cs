using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt;

public interface IRouteProvider
{
    IMqttTopicFilter[] TopicFilters { get; }

    Endpoint? GetEndpoint(IMqttRequestContext requestContext);
}
