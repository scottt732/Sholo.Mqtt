using System;
using Sholo.Mqtt.Old.ApplicationBuilder;
using Sholo.Mqtt.Old.ApplicationBuilderConfiguration;

namespace Sholo.Mqtt.Old.Test.Helpers
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
                b => b.WithTopic($"test/builder_{BuilderNumber}/build_{BuildCount++}"),
                MqttRequestDelegateHelper.Throws(() => new Exception($"{InvocationCount++}")));
        }
    }
}
