using MQTTnet.Packets;

namespace Sholo.Mqtt.Application;

public interface IMqttApplication
{
    MqttTopicFilter[] TopicFilters { get; }
    MqttRequestDelegate RequestDelegate { get; }
}
