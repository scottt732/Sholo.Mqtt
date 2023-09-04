using System;

namespace Sholo.Mqtt.TypeConverters;

public interface IMqttRequestStringTypeConverter : IMqttRequestBinaryTypeConverter
{
    bool TryConvertString(string sourceData, Type targetType, out object result);
}
