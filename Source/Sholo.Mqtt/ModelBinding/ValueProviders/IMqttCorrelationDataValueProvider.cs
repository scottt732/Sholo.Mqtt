namespace Sholo.Mqtt.ModelBinding.ValueProviders;

[PublicAPI]
public interface IMqttCorrelationDataValueProvider
{
    public byte[]? GetCorrelationData(IMqttModelBindingContext modelBindingContext, IMqttRequestContext requestContext);
}
