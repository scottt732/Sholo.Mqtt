namespace Sholo.Mqtt.ModelBinding.ValueProviders;

[PublicAPI]
public class MqttCorrelationDataValueProvider : IMqttCorrelationDataValueProvider
{
    public byte[]? GetValueSource(IMqttModelBindingContext mqttModelBindingContext) => mqttModelBindingContext.Request.CorrelationData;
}
