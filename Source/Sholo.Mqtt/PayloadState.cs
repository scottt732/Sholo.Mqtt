using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sholo.Mqtt;

public class PayloadState
{
    public bool IsValid => !ValidationResults.Any();
    public IList<ValidationResult> ValidationResults { get; } = new List<ValidationResult>();
}
