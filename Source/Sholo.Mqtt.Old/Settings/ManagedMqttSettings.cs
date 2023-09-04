using System;
using MQTTnet.Server;

namespace Sholo.Mqtt.Old.Settings
{
    public class ManagedMqttSettings : MqttSettings
    {
        public MqttPendingMessagesOverflowStrategy? PendingMessagesOverflowStrategy { get; set; }
        public TimeSpan? AutoReconnectDelay { get; set; }
        public int? MaxPendingMessages { get; set; }
    }
}