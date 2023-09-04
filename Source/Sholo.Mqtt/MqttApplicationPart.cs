using System.Reflection;

namespace Sholo.Mqtt
{
    public class MqttApplicationPart
    {
        public Assembly Assembly { get; }

        public MqttApplicationPart(Assembly assembly)
        {
            Assembly = assembly;
        }
    }
}
