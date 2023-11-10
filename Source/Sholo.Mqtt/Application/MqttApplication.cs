using System.Collections.Generic;
using System.Linq;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.Application;

internal class MqttApplication : IMqttApplication
{
    public IMqttTopicFilter[] TopicFilters { get; }
    public MqttRequestDelegate RequestDelegate { get; }

    public MqttApplication(IEnumerable<IMqttTopicFilter> topicFilters, MqttRequestDelegate requestDelegate)
    {
        TopicFilters = topicFilters.ToArray();
        RequestDelegate = requestDelegate;
    }
}
