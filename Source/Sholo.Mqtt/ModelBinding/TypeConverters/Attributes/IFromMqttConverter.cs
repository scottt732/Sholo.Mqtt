namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
public interface IFromMqttConverter
{
    MqttBindingSource BindingSource { get; }

    bool TryBind(IMqttRequestContext requestContext, ParameterState parameterState, out object? result);
}
