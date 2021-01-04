using System.Linq;
using Sholo.Mqtt.ApplicationBuilderConfiguration;
using Sholo.Mqtt.ApplicationProvider;

namespace Sholo.Mqtt.Test.Helpers
{
    public static class MqttApplicationProviderHelper
    {
        public static IMqttApplicationProvider CreateMqttApplicationProvider(int testBuilderCount)
        {
            var builders = Enumerable.Range(0, testBuilderCount)
                .Select(i => new TestConfigureMqttApplicationBuilder(i + 1))
                .Cast<IConfigureMqttApplicationBuilder>()
                .ToArray();

            var provider = new MqttApplicationProvider(builders);

            return provider;
        }
    }
}
