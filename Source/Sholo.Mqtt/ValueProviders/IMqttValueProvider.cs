namespace Sholo.Mqtt.ValueProviders
{
    public interface IMqttValueProvider<out TSourceType>
    {
        TSourceType GetValueSource(ParameterBindingContext context);
    }
}
