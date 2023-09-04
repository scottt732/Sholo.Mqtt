using System.Reflection;
using Sholo.Mqtt.Old.Topics.Filter;

namespace Sholo.Mqtt.Old.Topics.PatternFilter
{
    // See Bind() extension method on IMqttTopicFilter. This simplifies IMqttApplicationBuilder

    public interface IMqttTopicPatternFilter : IMqttTopicFilter
    {
        /// <summary>
        /// Gets the topic mask to subscribe to. This includes library-specific variable names that will be replaced when
        /// the actual topic is created for the subscription.  (e.g., <code>test/#topic_part/parts/*topic_parts</code>
        /// is equivalent to a <see cref="IMqttTopicFilter.Topic" /> value of <code>test/#/parts/*</code>.  The former
        /// is used for binding whereas the latter is MQTT-compliant)
        /// </summary>
        string TopicPattern { get; }

        bool IsMatch(string topic);

        void Bind(string topic, MethodInfo target);
    }
}
