using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.FilterBuilder;
using Sholo.Mqtt.Topics.PatternFilter;

namespace Sholo.Mqtt.Topics.PatternFilterBuilder;

public interface IMqttTopicPatternFilterBuilder
{
    /// <summary>
    /// The quality service level requested by the subscriber
    /// </summary>
    /// <param name="qualityOfServiceLevel">Quality of service level for subscription</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicPatternFilterBuilder WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel);

    /// <summary>
    /// The topic pattern to subscribe to.
    /// </summary>
    /// <param name="topicPattern">The topic or topic mask to subscribe to</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicPatternFilterBuilder WithTopicPattern(string topicPattern);

    /// <summary>
    /// Requests broker to not send application messages forwarded with a ClientID equal to the ClientID of the publishing connection.
    /// </summary>
    /// <remarks>
    /// This is only supported when using MQTTv5. See <a href="https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Subscription_Options">MQTT v5 Subscription Options</a>
    /// </remarks>
    /// <param name="noLocal">If <em>true</em>, broker would not send application messages with a ClientID equal to the clientID of the publishing connection.</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicPatternFilterBuilder WithNoLocal(bool noLocal);

    /// <summary>
    /// Indicates whether application messages forwarded using this subscription keep the retain flag they were published with.
    /// </summary>
    /// <remarks>
    /// This is only supported when using MQTTv5. See <a href="https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Subscription_Options">MQTT v5 Subscription Options</a>
    /// </remarks>
    /// <param name="retainAsPublished">If <em>true</em>, Application Messages forwarded using this subscription keep the RETAIN flag they were published with.
    /// If <em>false</em>, Application Messages forwarded using this subscription have the RETAIN flag set to 0.</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicPatternFilterBuilder WithRetainAsPublished(bool retainAsPublished);

    /// <summary>
    /// Configure whether retained messages are sent when the subscription is established.
    /// </summary>
    /// <remarks>
    /// This is only supported when using MQTTv5. See <a href="https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Subscription_Options">MQTT v5 Subscription Options</a>
    /// </remarks>
    /// <param name="retainHandling">Requests broker to send retained messages at the time of the subscribe, send retained messages at subscribe only if the subscription does not
    /// currently exist, or to not send retained subscribed messages.</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicPatternFilterBuilder WithRetainHandling(MqttRetainHandling retainHandling);

    /// <summary>
    /// Builds a <see cref="IMqttTopicPatternFilter" />
    /// </summary>
    /// <remarks>
    /// This is only supported when using MQTTv5. See <a href="https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Subscription_Options">MQTT v5 Subscription Options</a>
    /// </remarks>
    /// <returns>The built <see cref="IMqttTopicPatternFilter" /></returns>
    IMqttTopicPatternFilter Build();
}
