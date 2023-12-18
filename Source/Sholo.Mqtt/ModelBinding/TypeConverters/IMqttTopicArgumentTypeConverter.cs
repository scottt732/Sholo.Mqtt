using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

[PublicAPI]
public interface IMqttTopicArgumentTypeConverter : IMqttTypeConverter
{
    bool TryConvertTopicArgument(string argument, Type targetType, out object? result);
}
