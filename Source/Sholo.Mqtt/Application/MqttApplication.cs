using System;
using System.Collections.Generic;
using System.Linq;
using MQTTnet;

namespace Sholo.Mqtt.Application
{
    internal class MqttApplication : IMqttApplication
    {
        public MqttTopicFilter[] TopicFilters { get; }
        public MqttRequestDelegate RequestDelegate { get; }

        public MqttApplication(IEnumerable<MqttTopicFilter> topicFilters, MqttRequestDelegate requestDelegate)
        {
            TopicFilters = topicFilters?.ToArray() ?? Array.Empty<MqttTopicFilter>();
            RequestDelegate = requestDelegate;
        }
    }
}
