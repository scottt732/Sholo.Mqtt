using System;
using System.ComponentModel.DataAnnotations;
using MQTTnet.Formatter;

namespace Sholo.Mqtt.Settings;

[PublicAPI]
public class MqttSettings
{
    [Required]
    [MinLength(1)]
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool UseTls { get; set; }
    public bool IgnoreCertificateValidationErorrs { get; set; }
    public string ClientCertificatePublicKey { get; set; }
    public string ClientCertificatePrivateKey { get; set; }

    public int? Port { get; set; } = 1883;
    public string ClientId { get; set; }

    public MqttProtocolVersion? MqttProtocolVersion { get; set; } = MQTTnet.Formatter.MqttProtocolVersion.V500;
    public MqttMessageSettings OnlineMessage { get; set; } = new();
    public MqttMessageSettings LastWillAndTestament { get; set; } = new();

    public TimeSpan? Timeout { get; set; } = TimeSpan.FromSeconds(75);
    public TimeSpan? KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(30);
}
