using System;

namespace Sholo.Mqtt.TypeConverters;

[PublicAPI]
public interface IMqttParameterTypeConverter
{
    bool TryConvert(string value, Type targetType, out object? result);
}
