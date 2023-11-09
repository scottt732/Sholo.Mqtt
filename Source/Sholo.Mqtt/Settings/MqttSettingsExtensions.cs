using System;
using MQTTnet;

namespace Sholo.Mqtt.Settings;

public static class MqttSettingsExtensions
{
    public static MqttApplicationMessage? GetOnlineApplicationMessage(this MqttSettings mqttSettings)
    {
        ArgumentNullException.ThrowIfNull(mqttSettings, nameof(mqttSettings));

        return mqttSettings.OnlineMessage is { Topic: not null, Payload: not null }
            ? mqttSettings.OnlineMessage.ToMqttApplicationMessage()
            : null;
    }

    public static MqttApplicationMessage? GetLastWillAndTestamentApplicationMessage(this MqttSettings mqttSettings)
    {
        ArgumentNullException.ThrowIfNull(mqttSettings, nameof(mqttSettings));

        return mqttSettings.LastWillAndTestament is { Topic: not null, Payload: not null }
            ? mqttSettings.LastWillAndTestament.ToMqttApplicationMessage()
            : null;
    }
}
