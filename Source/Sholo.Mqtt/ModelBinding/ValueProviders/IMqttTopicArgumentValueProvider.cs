namespace Sholo.Mqtt.ModelBinding.ValueProviders;

[PublicAPI]
public interface IMqttTopicArgumentValueProvider : IMqttValueProvider
{
    string ParameterName { get; }
}
