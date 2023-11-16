using System.Reflection;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

public class MqttTopicArgumentValueProvider : IMqttTopicArgumentValueProvider
{
    public string ParameterName { get; }

    public MqttTopicArgumentValueProvider(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string[]? GetValueSource(IMqttModelBindingContext mqttModelBindingContext)
    {
        if (mqttModelBindingContext.TopicArguments.TryGetValue(ParameterName, out var values))
        {
            return values;
        }

        return null;
    }

    public MqttValueProviderResult GetValue(IMqttModelBindingContext mqttModelBindingContext, ParameterInfo actionParameter, out object? value)
    {
        throw new System.NotImplementedException();
    }
}
