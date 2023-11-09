using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.Application;

public interface IMqttApplication
{
    IMqttTopicFilter[] TopicFilters { get; }
    MqttRequestDelegate RequestDelegate { get; }
}
