using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MQTTnet.Server;

namespace Sholo.Mqtt.Settings;

[PublicAPI]
public class ManagedMqttSettings : MqttSettings
{
    /// <summary>
    ///     Gets or sets the maximum number of published messages waiting to be delivered to the broker before
    ///     the <see cref="PendingMessagesOverflowStrategy"/> is applied
    /// </summary>
    public int? MaxPendingMessages { get; set; }

    /// <summary>
    ///     Gets or sets a strategy that determines how to handle published messages when the number of queued
    ///     messages to be delivered exceeds <see cref="MaxPendingMessages"/> (default = <see cref="MqttPendingMessagesOverflowStrategy.DropNewMessage"/>)
    /// </summary>
    public MqttPendingMessagesOverflowStrategy? PendingMessagesOverflowStrategy { get; set; }

    /// <summary>
    ///     Gets or sets the delay between broker connection recovery attempts
    /// </summary>
    public TimeSpan? AutoReconnectDelay { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of topic filters in subscribe and unsubscribe packets.
    /// </summary>
    public int? MaxTopicFiltersInSubscribeUnsubscribePackets { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }

        if (MaxPendingMessages is <= 0)
        {
            yield return new ValidationResult($"{nameof(MaxPendingMessages)} must be greater than 0.", new[] { nameof(MaxPendingMessages) });
        }

        if (AutoReconnectDelay?.TotalSeconds < 0)
        {
            yield return new ValidationResult($"{nameof(AutoReconnectDelay)} must be greater than or equal to 0.", new[] { nameof(AutoReconnectDelay) });
        }

        if (MaxTopicFiltersInSubscribeUnsubscribePackets is < 0)
        {
            yield return new ValidationResult($"{nameof(MaxTopicFiltersInSubscribeUnsubscribePackets)} must be greater than or equal to 0.", new[] { nameof(MaxTopicFiltersInSubscribeUnsubscribePackets) });
        }
    }
}
