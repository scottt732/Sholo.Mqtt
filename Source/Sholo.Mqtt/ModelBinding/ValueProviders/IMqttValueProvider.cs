using System.Reflection;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

public interface IMqttValueProvider
{
    MqttValueProviderResult GetValue(IMqttModelBindingContext mqttModelBindingContext, IMqttRequestContext requestContext, ParameterInfo actionParameter, out object? value);
}
