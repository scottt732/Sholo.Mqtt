using System;

namespace Sholo.Mqtt.TypeConverters;

[PublicAPI]
public interface IMqttRequestPayloadTypeConverter
{
    bool TryConvertPayload(ArraySegment<byte> payloadData, Type targetType, out object? result);
}
