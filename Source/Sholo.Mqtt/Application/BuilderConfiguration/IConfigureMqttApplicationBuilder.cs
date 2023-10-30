using Sholo.Mqtt.Application.Builder;

namespace Sholo.Mqtt.Application.BuilderConfiguration;

public interface IConfigureMqttApplicationBuilder
{
    void ConfigureMqttApplication(IMqttApplicationBuilder mqttApplicationBuilder);
}
