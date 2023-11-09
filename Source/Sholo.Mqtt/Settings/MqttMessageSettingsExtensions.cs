using System;
using MQTTnet;

namespace Sholo.Mqtt.Settings;

public static class MqttMessageSettingsExtensions
{
    internal static MqttApplicationMessage ToMqttApplicationMessage(this MqttMessageSettings mqttMessageSettings)
    {
        ArgumentNullException.ThrowIfNull(mqttMessageSettings, nameof(mqttMessageSettings));

        var mqttMessageBuilder = new MqttApplicationMessageBuilder()
            .WithTopic(mqttMessageSettings.Topic)
            .WithPayload(mqttMessageSettings.Payload);

        if (mqttMessageSettings.QualityOfServiceLevel.HasValue)
        {
            mqttMessageBuilder = mqttMessageBuilder.WithQualityOfServiceLevel(mqttMessageSettings.QualityOfServiceLevel.Value);
        }

        if (mqttMessageSettings.Retain.HasValue)
        {
            mqttMessageBuilder = mqttMessageBuilder.WithRetainFlag(mqttMessageSettings.Retain.Value);
        }

        var mqttMessage = mqttMessageBuilder.Build();
        return mqttMessage;
    }
}
