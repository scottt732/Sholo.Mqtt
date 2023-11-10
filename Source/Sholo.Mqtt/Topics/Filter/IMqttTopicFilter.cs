using System.Collections.Generic;
using MQTTnet.Protocol;
using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.Topics.Filter;

[PublicAPI]
public interface IMqttTopicFilter
{
    /// <summary>
    ///     Gets the MQTT-compliant topic to be used for the subscription
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Gets the topic pattern used for binding data from inbound messages
    /// </summary>
    string TopicPattern { get; }

    /// <summary>
    ///     Gets the quality service level requested by the subscriber
    /// </summary>
    MqttQualityOfServiceLevel QualityOfServiceLevel { get; }

    /// <summary>
    ///     Gets a value indicating whether requests for the broker should not send application messages forwarded with a ClientID equal to the ClientID of the publishing connection.
    /// </summary>
    bool NoLocal { get; }

    /// <summary>
    ///     Gets a value indicating whether application messages forwarded using this subscription keep the retain flag they were published with.
    /// </summary>
    bool RetainAsPublished { get; }

    /// <summary>
    ///     Gets a flag that configures whether retained messages are sent when the subscription is established.
    /// </summary>
    MqttRetainHandling RetainHandling { get; }

    /// <summary>
    ///     Determines whether the incoming message's topic and quality of service level match this filter and extracts any topic arguments as strings.
    ///     Extracted arguments will be used to help find a matching <see cref="Endpoint" /> and attempt to bind the topic arguments to the endpoint's
    ///     parameters.
    /// </summary>
    /// <param name="context">
    ///     The <see cref="IMqttRequestContext" /> representing the incoming message. It provides a request-scoped <see cref="System.IServiceProvider" />
    /// </param>
    /// <param name="topicArguments">
    ///     The parameter names and argument values extracted from the topic using the <see cref="PatternMatcher.ITopicPatternMatcher" />
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the incoming message matches this topic filter.
    /// </returns>
    /// <remarks>
    ///     A true result does not guarantee that the message will be handled by an <see cref="Endpoint" />. It only indicates that the message is a
    ///     candidate for which binding should be attempted. See <see cref="RouteProvider" /> for the binding implementation &amp; behavior.
    /// </remarks>
    bool IsMatch(IMqttRequestContext context, out IReadOnlyDictionary<string, string[]>? topicArguments);
}
