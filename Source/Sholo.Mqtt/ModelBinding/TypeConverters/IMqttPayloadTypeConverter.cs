using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

[PublicAPI]
public interface IMqttPayloadTypeConverter : IMqttTypeConverter<ArraySegment<byte>>
{
}
