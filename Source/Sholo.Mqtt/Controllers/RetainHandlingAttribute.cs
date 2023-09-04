using System;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RetainHandlingAttribute : Attribute
    {
        public MqttRetainHandling RetainHandling { get; }

        public RetainHandlingAttribute(MqttRetainHandling retainHandling)
        {
            RetainHandling = retainHandling;
        }
    }
}
