using System;

namespace Sholo.Mqtt.TypeConverters;

public interface IMqttRequestPayloadTypeConverter
{
    bool TryConvertPayload(byte[] payloadData, Type targetType, out object result);
}
