using System;
using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.PatternMatcherFactory;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.Filter;

public class MqttTopicFilterTests
{
    [Theory]
    [InlineData(
        "test",
        MqttQualityOfServiceLevel.AtLeastOnce,
        false,
        true,
        MqttRetainHandling.SendAtSubscribe,
        "TopicFilter: [Topic=test] [QualityOfServiceLevel=AtLeastOnce] [NoLocal=False] [RetainAsPublished=True] [RetainHandling=SendAtSubscribe]")]
    [InlineData(
        "testing/1/2/3",
        MqttQualityOfServiceLevel.ExactlyOnce,
        true,
        false,
        MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly,
        "TopicFilter: [Topic=testing/1/2/3] [QualityOfServiceLevel=ExactlyOnce] [NoLocal=True] [RetainAsPublished=False] [RetainHandling=SendAtSubscribeIfNewSubscriptionOnly]")]
    public void ToString_ReturnsExpectedResults(
        string topic,
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        bool noLocal,
        bool retainAsPublished,
        MqttRetainHandling retainHandling,
        string expectedString
    )
    {
        var topicPatternMatcher = new TopicPatternMatcherFactory().CreateTopicPatternMatcher(topic);
        var mqttTopicFilter = new MqttTopicFilter(topicPatternMatcher, qualityOfServiceLevel, noLocal, retainAsPublished, retainHandling);
        var mqttTopicFilterStr = mqttTopicFilter.ToString();

        Assert.Equal(expectedString, mqttTopicFilterStr, StringComparer.Ordinal);
    }
}
