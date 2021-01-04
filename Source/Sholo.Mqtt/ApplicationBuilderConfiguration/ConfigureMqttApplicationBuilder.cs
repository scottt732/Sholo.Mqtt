using System;
using Sholo.Mqtt.ApplicationBuilder;

namespace Sholo.Mqtt.ApplicationBuilderConfiguration
{
    internal class ConfigureMqttApplicationBuilder : IConfigureMqttApplicationBuilder
    {
        private Action<IMqttApplicationBuilder> Configurator { get; }

        public ConfigureMqttApplicationBuilder(Action<IMqttApplicationBuilder> configurator)
        {
            Configurator = configurator;
        }

        public void ConfigureMqttApplication(IMqttApplicationBuilder mqttApplicationBuilder)
        {
            Configurator?.Invoke(mqttApplicationBuilder);
        }
    }
}
