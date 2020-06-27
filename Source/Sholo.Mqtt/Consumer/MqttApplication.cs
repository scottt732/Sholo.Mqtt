using System.Collections.ObjectModel;
using MQTTnet;

namespace Sholo.Mqtt.Consumer
{
    internal class MqttApplication : IMqttApplication
    {
        public ReadOnlyCollection<MqttTopicFilter> TopicFilters { get; }
        public MqttRequestDelegate RequestDelegate { get; }

        public MqttApplication(MqttTopicFilter[] topicFilters, MqttRequestDelegate requestDelegate)
        {
            TopicFilters = new ReadOnlyCollection<MqttTopicFilter>(topicFilters);
            RequestDelegate = requestDelegate;
        }
    }
}
