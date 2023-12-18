namespace Sholo.Mqtt.ModelBinding.ValueProviders;

/*
public class MqttTopicArgumentValueProvider : IMqttTopicArgumentValueProvider
{
    public string ParameterName { get; }

    public MqttTopicArgumentValueProvider(string parameterName)
    {
        ParameterName = parameterName;
    }

    public MqttValueProviderResult GetValue(IMqttModelBindingContext mqttModelBindingContext, IMqttRequestContext requestContext, ParameterInfo actionParameter, out object? value)
    {
        if (requestContext.TopicArguments.TryGetValue(ParameterName, out var values))
        {
            return values;
        }

        return null;
    }
}
*/
