using System.Collections.Generic;
using MQTTnet.Packets;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.Topics.PatternFilter;

// See Bind() extension method on IMqttTopicFilter. This simplifies IMqttApplicationBuilder

public interface IMqttTopicPatternFilter
{
    /// <summary>
    /// Gets the topic mask to subscribe to. This includes library-specific variable names that will be replaced when
    /// the actual topic is created for the subscription.  (e.g., <em>test/#topic_part/parts/*topic_parts</em>
    /// is equivalent to a <see cref="IMqttTopicFilter.Topic" /> value of <em>test/#/parts/*</em>.  The former
    /// is used for binding whereas the latter is MQTT-compliant)
    /// </summary>
    string TopicPattern { get; }

    string[] TopicParameterNames { get; }

    bool IsMatch(string topic, out IDictionary<string, string> topicArguments);

    MqttTopicFilter TopicFilter { get; }
}
