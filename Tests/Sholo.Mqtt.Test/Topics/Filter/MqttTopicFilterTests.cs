using System;
using System.Collections.Generic;
using Moq;
using MQTTnet.Protocol;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.PatternMatcher;
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

    [Theory]
    [CombinatorialData]
    public void IsMatch_WhenQualityOfServiceLevelMatches_UsesTopicPatternMatcher(
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        bool expectedResult)
    {
        const string expectedTopicPattern = "testing/+testing/#onetwothree";
        const string expectedTopic = "testing/+/#";

        var expectedTopicArguments = new Mock<IReadOnlyDictionary<string, string[]>>().Object;

        var mockContext = new Mock<IMqttRequestContext>(MockBehavior.Strict);

        mockContext
            .SetupGet(x => x.QualityOfServiceLevel)
            .Returns(qualityOfServiceLevel)
            .Verifiable();

        mockContext
            .SetupGet(x => x.Topic)
            .Returns(expectedTopic)
            .Verifiable();

        var mockTopicPatternMatcher = new Mock<ITopicPatternMatcher>(MockBehavior.Strict);

        mockTopicPatternMatcher
            .SetupGet(x => x.TopicPattern)
            .Returns(expectedTopicPattern)
            .Verifiable();

        mockTopicPatternMatcher
            .SetupGet(x => x.Topic)
            .Returns(expectedTopic)
            .Verifiable();

        mockTopicPatternMatcher
            .Setup(x => x.IsTopicMatch(expectedTopic, out expectedTopicArguments))
            .Returns(expectedResult)
            .Verifiable();

        var filter = new MqttTopicFilter(
            mockTopicPatternMatcher.Object,
            qualityOfServiceLevel,
            false,
            false,
            MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly
        );

        var result = filter.IsMatch(mockContext.Object, out var topicArguments);

        Assert.Equal(expectedResult, result);
        Assert.Same(expectedTopicArguments, topicArguments);
        Assert.Equal(expectedTopic, filter.Topic);
        Assert.Equal(expectedTopicPattern, filter.TopicPattern);
    }

    [Theory]
    [InlineData("test", MqttQualityOfServiceLevel.AtLeastOnce, MqttQualityOfServiceLevel.AtMostOnce)]
    [InlineData("test", MqttQualityOfServiceLevel.AtLeastOnce, MqttQualityOfServiceLevel.ExactlyOnce)]
    [InlineData("test", MqttQualityOfServiceLevel.AtMostOnce, MqttQualityOfServiceLevel.AtLeastOnce)]
    [InlineData("test", MqttQualityOfServiceLevel.AtMostOnce, MqttQualityOfServiceLevel.ExactlyOnce)]
    [InlineData("test", MqttQualityOfServiceLevel.ExactlyOnce, MqttQualityOfServiceLevel.AtLeastOnce)]
    [InlineData("test", MqttQualityOfServiceLevel.ExactlyOnce, MqttQualityOfServiceLevel.AtMostOnce)]
    public void IsMatch_WhenQualityOfServiceLevelDoesNotMatch_DoesNotUseTopicPatternMatcherAndReturnsFalse(
        string topic,
        MqttQualityOfServiceLevel topicFilterQualityOfServiceLevel,
        MqttQualityOfServiceLevel requestQualityOfServiceLevel
    )
    {
        var expectedTopicArguments = new Mock<IReadOnlyDictionary<string, string[]>>().Object;

        var mockContext = new Mock<IMqttRequestContext>(MockBehavior.Strict);

        mockContext
            .SetupGet(x => x.QualityOfServiceLevel)
            .Returns(requestQualityOfServiceLevel)
            .Verifiable();

        mockContext
            .SetupGet(x => x.Topic)
            .Returns(topic)
            .Verifiable();

        var mockTopicPatternMatcher = new Mock<ITopicPatternMatcher>(MockBehavior.Strict);
        mockTopicPatternMatcher
            .Setup(x => x.IsTopicMatch(It.IsAny<string>(), out expectedTopicArguments))
            .Verifiable(Times.Never);

        var filter = new MqttTopicFilter(
            mockTopicPatternMatcher.Object,
            topicFilterQualityOfServiceLevel,
            false,
            false,
            MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly
        );

        var result = filter.IsMatch(mockContext.Object, out var topicArguments);

        Assert.False(result);
        Assert.Null(topicArguments);
    }
}
