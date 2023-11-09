#nullable enable

using System;

namespace Sholo.Mqtt.TypeConverters.Payload;

public class DefaultPayloadTypeConverter : IMqttRequestPayloadTypeConverter
{
    public bool TryConvertPayload(ArraySegment<byte> payloadData, Type targetType, out object? result) => DefaultTypeConverters.TryConvert(payloadData, targetType, out result);
}
