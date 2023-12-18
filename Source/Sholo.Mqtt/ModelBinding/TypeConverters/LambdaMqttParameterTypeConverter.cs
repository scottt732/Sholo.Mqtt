using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

internal class LambdaMqttParameterTypeConverter<TTargetType> : IMqttUserPropertiesTypeConverter
{
    private Func<StringValues?, (bool Success, TTargetType? Result)> Converter { get; }

    public LambdaMqttParameterTypeConverter(Func<StringValues?, (bool Success, TTargetType? Result)> converter)
    {
        Converter = converter;
    }

    public bool TryConvertUserPropertyValues(StringValues? values, Type targetType, out IList<object?>? result)
    {
        var results = new List<object?>();
        try
        {
            var allSuccess = true;
            var (success, typedResult) = Converter.Invoke(values);
            if (success)
            {
                results.Add(typedResult);
            }
            else
            {
                allSuccess = false;
            }

            if (allSuccess)
            {
                result = results;
                return true;
            }

            result = null;
            return false;
        }
        catch
        {
            result = null;
            return false;
        }
    }
}
