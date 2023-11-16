using System.Reflection;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;
public interface IMqttValueProvider
{
    MqttValueProviderResult GetValue(IMqttModelBindingContext mqttModelBindingContext, ParameterInfo actionParameter, out object? value);
}
