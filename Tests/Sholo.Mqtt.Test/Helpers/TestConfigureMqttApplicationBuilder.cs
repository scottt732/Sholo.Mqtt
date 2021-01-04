using System;
using Sholo.Mqtt.ApplicationBuilder;
using Sholo.Mqtt.ApplicationBuilderConfiguration;

namespace Sholo.Mqtt.Test.Helpers
{
    public class TestConfigureMqttApplicationBuilder : IConfigureMqttApplicationBuilder
    {
        private int BuilderNumber { get; }
        private int BuildCount { get; set; } = 1;
        private int InvocationCount { get; set; } = 1;

        public TestConfigureMqttApplicationBuilder(int builderNumber)
        {
            BuilderNumber = builderNumber;
        }

        public void ConfigureMqttApplication(IMqttApplicationBuilder mqttApplicationBuilder)
        {
            mqttApplicationBuilder.Map(
                $"test/builder_{BuilderNumber}/build_{BuildCount++}",
                MqttRequestDelegateHelper.Throws(() => new Exception($"{InvocationCount++}")));
        }
    }
}
