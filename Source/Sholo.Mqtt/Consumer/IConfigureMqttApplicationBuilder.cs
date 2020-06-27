namespace Sholo.Mqtt.Consumer
{
    public interface IConfigureMqttApplicationBuilder
    {
        void Configure(IMqttApplicationBuilder mqttApplicationBuilder);
    }
}