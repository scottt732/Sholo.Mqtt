using System;

namespace Sholo.Mqtt.ValueProviders;

public class MqttPayloadValueProvider : IMqttValueProvider<ArraySegment<byte>>
{
    public ArraySegment<byte> GetValueSource(ParameterBindingContext context) => context.Request.Payload;
}
