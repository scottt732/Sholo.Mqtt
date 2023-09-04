using System;

namespace Sholo.Mqtt.TypeConverters;

public interface IMqttRequestBinaryTypeConverter
{
    bool TryConvertBinary(byte[] sourceData, Type targetType, out object result);
}
