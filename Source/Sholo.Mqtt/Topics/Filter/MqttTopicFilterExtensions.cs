namespace Sholo.Mqtt.Topics.Filter;

internal static class MqttTopicFilterExtensions
{
    public static MQTTnet.Packets.MqttTopicFilter ToMqttNetTopicFilter(this IMqttTopicFilter mqttTopicFilter)
    {
        return new MQTTnet.Packets.MqttTopicFilter()
        {
            Topic = mqttTopicFilter.Topic,
            QualityOfServiceLevel = mqttTopicFilter.QualityOfServiceLevel,
            RetainHandling = mqttTopicFilter.RetainHandling,
            RetainAsPublished = mqttTopicFilter.RetainAsPublished,
            NoLocal = mqttTopicFilter.NoLocal
        };
    }
}
