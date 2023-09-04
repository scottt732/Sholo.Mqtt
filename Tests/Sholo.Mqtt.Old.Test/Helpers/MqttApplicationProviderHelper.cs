using System.Linq;
using Sholo.Mqtt.Old.ApplicationBuilderConfiguration;
using Sholo.Mqtt.Old.ApplicationProvider;

namespace Sholo.Mqtt.Old.Test.Helpers
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
