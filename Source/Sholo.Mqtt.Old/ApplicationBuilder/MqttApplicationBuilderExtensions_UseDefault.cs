using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sholo.Mqtt.Old.ApplicationBuilder
{
    // ReSharper disable once InconsistentNaming
    [PublicAPI]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Extension methods")]
    public static class MqttApplicationBuilderExtensions_UseDefault
    {
        public static IMqttApplicationBuilder UseDefault(this IMqttApplicationBuilder mqttApplicationBuilder)
        {
            mqttApplicationBuilder.Use(_ => Task.FromResult(true));
            return mqttApplicationBuilder;
        }
    }
}
