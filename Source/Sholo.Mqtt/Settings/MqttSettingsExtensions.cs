using MQTTnet;

namespace Sholo.Mqtt.Settings;

public static class MqttSettingsExtensions
{
    public static MqttApplicationMessage GetOnlineApplicationMessage(this MqttSettings settings)
    {
        return settings?.OnlineMessage?.Topic != null && settings.OnlineMessage?.Payload != null
            ? BuildApplicationMessage(settings.OnlineMessage)
            : null;
    }

    public static MqttApplicationMessage GetLastWillAndTestamentApplicationMessage(this MqttSettings settings)
    {
        return settings?.LastWillAndTestament?.Topic != null && settings.LastWillAndTestament?.Payload != null
            ? BuildApplicationMessage(settings.LastWillAndTestament)
            : null;
    }

    private static MqttApplicationMessage BuildApplicationMessage(MqttMessageSettings mqttMessageSettings)
    {
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
