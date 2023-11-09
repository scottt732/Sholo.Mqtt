using System.ComponentModel.DataAnnotations;

namespace Mqtt.Sample.Models;

// test/run/some_user/25/2
// {"hello":"world","test":"123"}
[PublicAPI]
public class RunModel
{
    public string Hello { get; set; }

    [Required]
    public string Test { get; set; }
}
