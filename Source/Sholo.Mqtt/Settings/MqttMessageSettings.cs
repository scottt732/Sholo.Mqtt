#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.Settings;

[PublicAPI]
public class MqttMessageSettings : IValidatableObject
{
    /// <summary>
    ///     Gets or sets the MQTT topic to publish the message to
    /// </summary>
    public string Topic { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the MQTT message payload to publish
    /// </summary>
    public string Payload { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the quality of service level with which to publish the message (default = <see cref="MqttQualityOfServiceLevel.AtMostOnce"/> or 0)
    /// </summary>
    public MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether or not the published message should be retained by the broker (default = false)
    /// </summary>
    public bool? Retain { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Topic == null!)
        {
            yield return new ValidationResult($"{nameof(Topic)} is required.", new[] { nameof(Topic) });
        }
        else if (Topic.Length == 0)
        {
            yield return new ValidationResult($"{nameof(Topic)} can not be empty.", new[] { nameof(Topic) });
        }
        else if (Topic.Any(c => c is '+' or '#'))
        {
            yield return new ValidationResult("The characters '+' and '#' are not allowed in topics.", new[] { nameof(Topic) });
        }

        if (QualityOfServiceLevel.HasValue && !Enum.IsDefined(typeof(MqttQualityOfServiceLevel), QualityOfServiceLevel.Value))
        {
            yield return new ValidationResult($"Invalid {nameof(QualityOfServiceLevel)} value: {QualityOfServiceLevel.Value}", new[] { nameof(QualityOfServiceLevel) });
        }

        if (Payload == null!)
        {
            yield return new ValidationResult($"{nameof(Payload)} is required.", new[] { nameof(Payload) });
        }
        else if (Payload.Length == 0)
        {
            yield return new ValidationResult($"{nameof(Payload)} can not be empty.", new[] { nameof(Payload) });
        }
    }
}
