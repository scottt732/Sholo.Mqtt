using System.Reflection;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

public class MqttPayloadValueProvider : IMqttPayloadValueProvider
{
    public MqttValueProviderResult GetValue(IMqttModelBindingContext mqttModelBindingContext, IMqttRequestContext requestContext, ParameterInfo actionParameter, out object? value)
    {
        value = requestContext.Payload;
        return MqttValueProviderResult.None;
    }
}
