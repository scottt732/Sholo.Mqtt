using Sholo.Mqtt.ApplicationBuilder;

namespace Sholo.Mqtt.ApplicationBuilderConfiguration
{
    public interface IConfigureMqttApplicationBuilder
    {
        void ConfigureMqttApplication(IMqttApplicationBuilder mqttApplicationBuilder);
    }
}
