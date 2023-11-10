using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.ValueProviders;

[PublicAPI]
public interface IMqttValueProvider<out TSourceType>
{
    public TSourceType GetValueSource(IParameterBindingContext context);
}
