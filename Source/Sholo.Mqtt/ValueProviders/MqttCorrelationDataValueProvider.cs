namespace Sholo.Mqtt.ValueProviders;

public class MqttCorrelationDataValueProvider : IMqttValueProvider<byte[]>
{
    public byte[] GetValueSource(ParameterBindingContext context) => context.Request.CorrelationData;
}
