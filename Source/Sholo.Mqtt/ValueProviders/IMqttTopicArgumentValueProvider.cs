namespace Sholo.Mqtt.ValueProviders;

[PublicAPI]
public interface IMqttTopicArgumentValueProvider : IMqttValueProvider<string[]?>
{
    string ParameterName { get; }
}
