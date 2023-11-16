using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

internal class LambdaMqttParameterTypeConverter<TTargetType> : IMqttParameterTypeConverter
{
    private Func<string?, (bool Success, TTargetType? Result)> Converter { get; }

    public LambdaMqttParameterTypeConverter(Func<string?, (bool Success, TTargetType? Result)> converter)
    {
        Converter = converter;
    }

    public bool TryConvertParameter(string? value, Type targetType, out object? result)
    {
        try
        {
            var (success, typedResult) = Converter.Invoke(value);
            result = typedResult;
            return success;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
