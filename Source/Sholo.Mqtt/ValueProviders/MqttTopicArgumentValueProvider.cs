namespace Sholo.Mqtt.ValueProviders;

public class MqttTopicArgumentValueProvider : IMqttValueProvider<string>
{
    public string ParameterName { get; }

    public MqttTopicArgumentValueProvider(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string GetValueSource(ParameterBindingContext context)
    {
        return context.TopicArguments.TryGetValue(ParameterName, out var value) ? value : null;
    }
}
