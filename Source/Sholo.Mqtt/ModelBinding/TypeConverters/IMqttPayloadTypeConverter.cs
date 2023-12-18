using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

[PublicAPI]
public interface IMqttPayloadTypeConverter : IMqttTypeConverter
{
    bool TryConvertPayload(ArraySegment<byte> payload, Type targetType, out object? result);
}
