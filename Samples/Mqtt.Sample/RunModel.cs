using System.ComponentModel.DataAnnotations;

namespace Mqtt.Sample;

// test/run/someuser/25/2
// {"hello":"world","test":"123"}
[PublicAPI]
public class RunModel
{
    public string Hello { get; set; }

    [Required]
    public string Test { get; set; }
}
