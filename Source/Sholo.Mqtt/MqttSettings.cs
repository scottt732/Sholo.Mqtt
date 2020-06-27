using System;
using System.ComponentModel.DataAnnotations;
using MQTTnet.Formatter;

namespace Sholo.Mqtt
{
    public class MqttSettings
    {
        [Required]
        [MinLength(1)]
        public string Host { get; set; }

        public int? Port { get; set; } = 1883;
        public string ClientId { get; set; }

        public MqttProtocolVersion? MqttProtocolVersion { get; set; } = MQTTnet.Formatter.MqttProtocolVersion.V500;
        public MqttMessageSettings OnlineMessage { get; set; } = new MqttMessageSettings();
        public MqttMessageSettings LastWillAndTestament { get; set; } = new MqttMessageSettings();

        public TimeSpan? CommunicationTimeout { get; set; } = TimeSpan.FromSeconds(75);
        public TimeSpan? KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(30);
    }
}
