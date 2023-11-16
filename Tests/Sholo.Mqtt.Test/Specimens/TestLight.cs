using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Sholo.Mqtt.Test.Specimens;

[PublicAPI]
public class TestLight : IValidatableObject
{
    private static readonly DateTimeOffset HomeAutomationEpoch = new(2013, 9, 17, 0, 32, 51, TimeSpan.FromHours(-7));

    [Required]
    [MinLength(2)]
    public string Id { get; set; } = null!;
    public TestLightState State { get; set; }
    public bool Online { get; set; }

    [Required]
    public DateTimeOffset? LastUpdated { get; set; }

    [Required]
#pragma warning disable CA2227
    public Dictionary<string, string> Attributes { get; set; } = null!;
#pragma warning restore CA2227

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LastUpdated < HomeAutomationEpoch)
        {
            yield return new ValidationResult("Light updates from the Dark Ages aren't relevant", new[] { nameof(LastUpdated) });
        }
    }
}
