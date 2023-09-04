using System;
using Sholo.Mqtt.Old.ApplicationBuilder;

namespace Sholo.Mqtt.Old.ApplicationBuilderConfiguration
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
