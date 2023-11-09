#nullable enable

using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.ValueProviders;

public class MqttTopicArgumentValueProvider : IMqttTopicArgumentValueProvider
{
    public string ParameterName { get; }

    public MqttTopicArgumentValueProvider(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string[]? GetValueSource(IParameterBindingContext context)
    {
        if (context.TopicArguments?.TryGetValue(ParameterName, out var values) ?? false)
        {
            return values;
        }

        return null;
    }
}
