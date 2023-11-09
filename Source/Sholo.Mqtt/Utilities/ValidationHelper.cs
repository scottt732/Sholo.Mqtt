#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sholo.Mqtt.Utilities;

[PublicAPI]
internal static class ValidationHelper
{
    public static IEnumerable<ValidationResult> GetValidationResults(object obj)
    {
        TryValidateObject(obj, out var results);
        foreach (var result in results)
        {
            yield return result;
        }
    }

    public static bool TryValidateObject(object obj, out ValidationResult[] results)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var validationResults = new List<ValidationResult>();
        var success = ValidateObject(obj, validationResults);

        results = validationResults.ToArray();
        return success;
    }

    public static bool ValidateObject(object obj, IList<ValidationResult> validationResults)
    {
        var validationContext = new ValidationContext(obj);

        var success = Validator.TryValidateObject(
            obj,
            validationContext,
            validationResults,
            validateAllProperties: true
        );

        /*
        if (obj is IValidatableObject validatableObject)
        {
            ValidateValidatableObject(validationContext, validatableObject, validationResults);
            success = validationResults.Count == 0;
        }
        */

        return success;
    }

    private static void ValidateValidatableObject(ValidationContext validationContext, IValidatableObject validatableObject, IList<ValidationResult> validationResults)
    {
        var ungroupedResults = validatableObject.Validate(validationContext);

        foreach (var result in ungroupedResults
                     .GroupBy(x => string.Join(",", x.MemberNames?.OrderBy(y => y).ToArray() ?? Array.Empty<string>()))
                     .Select(x => x.GroupBy(y => (y.MemberNames, y.ErrorMessage)).Select(y => y.Key))
                     .SelectMany(x => x)
                     .Select(x => new ValidationResult(x.ErrorMessage, x.MemberNames)))
        {
            validationResults.Add(result);
        }
    }
}
