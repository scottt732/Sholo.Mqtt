namespace Sholo.Mqtt.ModelBinding.ValueProviders;

[PublicAPI]
public class MqttCorrelationDataValueProvider : IMqttCorrelationDataValueProvider
{
    public byte[]? GetCorrelationData(IMqttModelBindingContext modelBindingContext, IMqttRequestContext requestContext) => requestContext.CorrelationData;
}
