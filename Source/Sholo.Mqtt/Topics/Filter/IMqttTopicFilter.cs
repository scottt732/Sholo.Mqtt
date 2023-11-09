#nullable enable
using System.Collections.Generic;
using MQTTnet.Protocol;
using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.Topics.Filter;

[PublicAPI]
public interface IMqttTopicFilter
{
    /// <summary>
    /// Gets the topic for the subscription
    /// </summary>
    string Topic { get; }

    /// <summary>
    /// Gets the quality service level requested by the subscriber
    /// </summary>
    MqttQualityOfServiceLevel QualityOfServiceLevel { get; }

    /// <summary>
    /// Gets a value indicating whether requests for the broker should not send application messages forwarded with a ClientID equal to the ClientID of the publishing connection.
    /// </summary>
    bool NoLocal { get; }

    /// <summary>
    /// Gets a value indicating whether application messages forwarded using this subscription keep the retain flag they were published with.
    /// </summary>
    bool RetainAsPublished { get; }

    /// <summary>
    /// Gets a flag that configures whether retained messages are sent when the subscription is established.
    /// </summary>
    MqttRetainHandling RetainHandling { get; }

    bool IsMatch(IMqttRequestContext context, out IReadOnlyDictionary<string, string[]>? topicArguments);
}
