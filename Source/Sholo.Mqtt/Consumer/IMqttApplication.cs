using System.Collections.ObjectModel;
using MQTTnet;

namespace Sholo.Mqtt.Consumer
{
    public interface IMqttApplication
    {
        ReadOnlyCollection<MqttTopicFilter> TopicFilters { get; }
        MqttRequestDelegate RequestDelegate { get; }
    }
}
