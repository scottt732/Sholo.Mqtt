using System;

namespace Sholo.Mqtt
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class MqttApplicationPartAttribute : Attribute
    {
        public MqttApplicationPartAttribute()
        {
        }
    }
}
