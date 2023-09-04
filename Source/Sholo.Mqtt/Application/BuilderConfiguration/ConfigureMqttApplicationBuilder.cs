using System;
using Sholo.Mqtt.Application.Builder;

namespace Sholo.Mqtt.Application.BuilderConfiguration
{
    internal class ConfigureMqttApplicationBuilder : IConfigureMqttApplicationBuilder
    {
        private Action<IMqttApplicationBuilder> Configure { get; }

        public ConfigureMqttApplicationBuilder(Action<IMqttApplicationBuilder> configure)
        {
            Configure = configure;
        }

        public void ConfigureMqttApplication(IMqttApplicationBuilder mqttApplicationBuilder)
        {
            Configure?.Invoke(mqttApplicationBuilder);
        }
    }
}
