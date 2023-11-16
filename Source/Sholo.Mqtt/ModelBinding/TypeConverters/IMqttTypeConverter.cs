using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

[PublicAPI]
public interface IMqttTypeConverter<in TInput>
{
    bool TryConvert(TInput? input, Type targetType, out object? result);
}
