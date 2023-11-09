#nullable enable

using System;
using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.ValueProviders;

public class MqttPayloadValueProvider : IMqttPayloadValueProvider
{
    public ArraySegment<byte> GetValueSource(IParameterBindingContext context) => context.Request.Payload;
}
