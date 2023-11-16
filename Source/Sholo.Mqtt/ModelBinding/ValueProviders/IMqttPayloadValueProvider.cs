using System;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

[PublicAPI]
public interface IMqttPayloadValueProvider : IMqttValueProvider<ArraySegment<byte>>
{
}
