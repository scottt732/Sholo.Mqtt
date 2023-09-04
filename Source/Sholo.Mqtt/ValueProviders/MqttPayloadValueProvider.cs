namespace Sholo.Mqtt.ValueProviders;

public class MqttPayloadValueProvider : IMqttValueProvider<byte[]>
{
    public byte[] GetValueSource(ParameterBindingContext context) => context.Request.Payload;
}
