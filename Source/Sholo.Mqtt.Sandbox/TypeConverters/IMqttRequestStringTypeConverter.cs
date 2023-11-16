using System;

namespace Sholo.Mqtt.TypeConverters;

[PublicAPI]
public interface IMqttRequestStringTypeConverter : IMqttRequestBinaryTypeConverter
{
    bool TryConvertString(string sourceData, Type targetType, out object result);
}
