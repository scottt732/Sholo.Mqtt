using Sholo.Mqtt.Old.ApplicationBuilder;

namespace Sholo.Mqtt.Old.ApplicationBuilderConfiguration
{
    public interface IConfigureMqttApplicationBuilder
    {
        void ConfigureMqttApplication(IMqttApplicationBuilder mqttApplicationBuilder);
    }
}
