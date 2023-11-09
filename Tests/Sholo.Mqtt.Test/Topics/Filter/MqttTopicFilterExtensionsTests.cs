using System;
using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.PatternMatcherFactory;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.Filter;

public class MqttTopicFilterExtensionsTests
{
    [Theory]
    [InlineData(
        "test",
        MqttQualityOfServiceLevel.AtLeastOnce,
        false,
        true,
        MqttRetainHandling.SendAtSubscribe)]
    [InlineData(
        "testing/1/2/3",
        MqttQualityOfServiceLevel.ExactlyOnce,
        true,
        false,
        MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly)]
    public void ToMqttNetTopicFilter_WhenInvoked_ResultHasExpectedValues(
        string topic,
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        bool noLocal,
        bool retainAsPublished,
        MqttRetainHandling retainHandling)
    {
        var topicPatternMatcher = new TopicPatternMatcherFactory().CreateTopicPatternMatcher(topic);
        var mqttTopicFilter = new MqttTopicFilter(topicPatternMatcher, qualityOfServiceLevel, noLocal, retainAsPublished, retainHandling);
        var mqttNetTopicFilter = mqttTopicFilter.ToMqttNetTopicFilter();

        Assert.Equal(topic, mqttNetTopicFilter.Topic, StringComparer.Ordinal);
        Assert.Equal(qualityOfServiceLevel, mqttNetTopicFilter.QualityOfServiceLevel);
        Assert.Equal(noLocal, mqttNetTopicFilter.NoLocal);
        Assert.Equal(retainAsPublished, mqttNetTopicFilter.RetainAsPublished);
        Assert.Equal(retainHandling, mqttNetTopicFilter.RetainHandling);
    }
}
