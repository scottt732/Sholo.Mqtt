#nullable enable

using System;

namespace Sholo.Mqtt.ValueProviders;

[PublicAPI]
public interface IMqttPayloadValueProvider : IMqttValueProvider<ArraySegment<byte>>
{
}
