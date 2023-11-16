using System;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

public class MqttPayloadValueProvider : IMqttPayloadValueProvider
{
    public ArraySegment<byte> GetValueSource(IMqttModelBindingContext mqttModelBindingContext) => mqttModelBindingContext.Request.Payload;
}
