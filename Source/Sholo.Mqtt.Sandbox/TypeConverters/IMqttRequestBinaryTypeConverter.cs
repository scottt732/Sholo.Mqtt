using System;

namespace Sholo.Mqtt.TypeConverters;

[PublicAPI]
public interface IMqttRequestBinaryTypeConverter
{
    bool TryConvertBinary(byte[] sourceData, Type targetType, out object result);
}
