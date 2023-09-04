using MQTTnet;

namespace Sholo.Mqtt.Old.Application
{
    public interface IMqttApplication
    {
        MqttTopicFilter[] TopicFilters { get; }
        MqttRequestDelegate RequestDelegate { get; }
    }
}
