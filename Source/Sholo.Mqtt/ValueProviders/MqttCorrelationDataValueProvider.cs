using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.ValueProviders;

public class MqttCorrelationDataValueProvider : IMqttCorrelationDataValueProvider
{
    public byte[]? GetValueSource(IParameterBindingContext context) => context.Request.CorrelationData;
}
