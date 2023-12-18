using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.Topics.FilterBuilder;

[PublicAPI]
public interface IMqttTopicFilterBuilder
{
    /// <summary>
    /// The quality service level requested by the subscriber
    /// </summary>
    /// <param name="qualityOfServiceLevel">Quality of service level for subscription</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicFilterBuilder WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel);

    /// <summary>
    /// The topic (or topic mask) to subscribe to. The builder will remove Sholo.Mqtt library parameter variable names from the mask
    /// (e.g., <em>test/#topic_part/parts/*topic_parts</em> will be changed to <em>test/#/parts/*</em>)
    /// </summary>
    /// <param name="topicPattern">
    /// The topic or topic mask to subscribe to
    /// </param>
    /// <returns>
    /// The <see cref="IMqttTopicFilterBuilder" /> being configured
    /// </returns>
    IMqttTopicFilterBuilder WithTopicPattern(string topicPattern); // TODO: bool caseSensitive

    /// <summary>
    /// Requests broker to not send application messages forwarded with a ClientID equal to the ClientID of the publishing connection.
    /// </summary>
    /// <remarks>
    /// This is only supported when using MQTTv5. See <a href="https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Subscription_Options">MQTT v5 Subscription Options</a>
    /// </remarks>
    /// <param name="noLocal">If <em>true</em>, broker would not send application messages with a ClientID equal to the clientID of the publishing connection.</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicFilterBuilder WithNoLocal(bool noLocal);

    /// <summary>
    /// Indicates whether application messages forwarded using this subscription keep the retain flag they were published with.
    /// </summary>
    /// <remarks>
    /// This is only supported when using MQTTv5. See <a href="https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Subscription_Options">MQTT v5 Subscription Options</a>
    /// </remarks>
    /// <param name="retainAsPublished">If <em>true</em>, Application Messages forwarded using this subscription keep the RETAIN flag they were published with.
    /// If <em>false</em>, Application Messages forwarded using this subscription have the RETAIN flag set to 0.</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicFilterBuilder WithRetainAsPublished(bool retainAsPublished);

    /// <summary>
    /// Configure whether retained messages are sent when the subscription is established.
    /// </summary>
    /// <remarks>
    /// This is only supported when using MQTTv5. See <a href="https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Subscription_Options">MQTT v5 Subscription Options</a>
    /// </remarks>
    /// <param name="retainHandling">Requests broker to send retained messages at the time of the subscribe, send retained messages at subscribe only if the subscription does not
    /// currently exist, or to not send retained subscribed messages.</param>
    /// <returns>The <see cref="IMqttTopicFilterBuilder" /> being configured</returns>
    IMqttTopicFilterBuilder WithRetainHandling(MqttRetainHandling retainHandling);

    IMqttTopicFilter Build();
}
