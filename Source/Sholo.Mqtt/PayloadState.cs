using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sholo.Mqtt;

public class PayloadState
{
    public bool IsValid => !Results.Any();
    public IList<ValidationResult> Results { get; }

    public PayloadState()
    {
        Results = new List<ValidationResult>();
    }
}
