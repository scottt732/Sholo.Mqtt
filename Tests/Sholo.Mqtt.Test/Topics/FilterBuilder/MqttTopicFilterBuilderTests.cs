using System;
using Moq;
using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.FilterBuilder;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.FilterBuilder;

public class MqttTopicFilterBuilderTests
{
    [Theory]
    [InlineData("+")]
    [InlineData("testing/+")]
    [InlineData("is/this/+/on")]
    [InlineData("is/this/+/+state")]
    [InlineData("#")]
    [InlineData("testing/testing/#")]
    [InlineData("testing/+device/#")]
    [InlineData("testing/+/#wildcard")]
    public void Build_WhenTopicPatternIsComplexAndHasMissingVariableNames_ThrowsArgumentException(string complexTopicPattern)
    {
        var ane = Assert.Throws<ArgumentException>(() => new MqttTopicFilterBuilder().WithTopicPattern(complexTopicPattern).Build());

        Assert.StartsWith(
            "The topic pattern specified must contain a variable name after + or # characters (e.g., Test/+Username/#Options). This will be translated to an MQTT subscription (Test/+/#) and the values will be used to expose named properties",
            ane.Message,
            StringComparison.Ordinal
        );
        Assert.Equal("topicPattern", ane.ParamName, StringComparer.Ordinal);
    }

    [Theory]
    [InlineData("#invalid/+test")]
    [InlineData("testing/#invalid/+test")]
    [InlineData("are/these/#devices/on/is/invalid")]
    [InlineData("are/these/#devices/+state/is/invalid")]
    [InlineData("#wildcard/now_invalid")]
    [InlineData("testing/#devices/#numbers")]
    [InlineData("testing/#devices/+number")]
    public void Build_WhenTopicWildcardVariableAppearsInMiddleSegment_ThrowsArgumentException(string complexTopicPattern)
    {
        var ae = Assert.Throws<ArgumentException>(() => new MqttTopicFilterBuilder().WithTopicPattern(complexTopicPattern).Build());

        Assert.StartsWith("Multi-level wildcards can only appear at the end of a topic pattern.", ae.Message, StringComparison.Ordinal);
        Assert.Equal("topicPattern", ae.ParamName, StringComparer.Ordinal);
    }

    [Fact]
    public void Build_WhenTopicIsNull_ThrowsArgumentNullException()
    {
        var ane = Assert.Throws<ArgumentNullException>(() => new MqttTopicFilterBuilder().Build());

        Assert.StartsWith("topicPattern is required", ane.Message, StringComparison.Ordinal);
        Assert.Equal("topicPattern", ane.ParamName);
    }

    [Fact]
    public void Build_WhenTopicIsEmpty_ThrowsArgumentNullException()
    {
        var ae = Assert.Throws<ArgumentException>(() => new MqttTopicFilterBuilder().WithTopicPattern(string.Empty).Build());

        Assert.StartsWith("topicPattern must be non-empty", ae.Message, StringComparison.Ordinal);
        Assert.Equal("topicPattern", ae.ParamName);
    }

    [Fact]
    public void Build_WhenTopicPatternIsValid_ReturnsExpectedResult()
    {
        var mqttTopicFilter = new MqttTopicFilterBuilder().WithTopicPattern("testing/+action/#numbers").Build();
        var expectedMatch = "testing/testing/1/2/3";
        var expectedMismatch = "mismatch/mismatch/mismatch/mismatch/mismatch";

        var mockExpectedMatchContext = new Mock<IMqttRequestContext>();

        mockExpectedMatchContext
            .SetupGet(x => x.Topic)
            .Returns(expectedMatch);

        var expectedMatchContext = mockExpectedMatchContext.Object;

        var mockExpectedMismatchContext = new Mock<IMqttRequestContext>();

        mockExpectedMismatchContext
            .SetupGet(x => x.Topic)
            .Returns(expectedMismatch);

        var expectedMismatchContext = mockExpectedMismatchContext.Object;

        var expectedMatchIsMatch = mqttTopicFilter.IsMatch(expectedMatchContext, out var expectedMatchTopicArguments);
        var expectedMismatchIsMatch = mqttTopicFilter.IsMatch(expectedMismatchContext, out var expectedMismatchTopicArguments);

        Assert.NotNull(mqttTopicFilter);

        Assert.True(expectedMatchIsMatch);
        Assert.NotNull(expectedMatchTopicArguments);
        Assert.Collection(
            expectedMatchTopicArguments,
            kvp =>
            {
                Assert.Equal("action", kvp.Key);
                Assert.Collection(
                    kvp.Value,
                    a => Assert.Equal("testing", a)
                );
            },
            kvp =>
            {
                Assert.Equal("numbers", kvp.Key);
                Assert.Collection(
                    kvp.Value,
                    a => Assert.Equal("1", a),
                    a => Assert.Equal("2", a),
                    a => Assert.Equal("3", a)
                );
            }
        );

        Assert.False(expectedMismatchIsMatch);
        Assert.Null(expectedMismatchTopicArguments);
    }

    [Theory]
    [CombinatorialData]
    public void Build_WhenTopicPatternIsValidWithOtherArgs_ReturnsExpectedResult(MqttQualityOfServiceLevel qualityOfServiceLevel, bool noLocal, bool retainAsPublished, MqttRetainHandling retainHandling)
    {
        const string topicPattern = "testing/+action/#numbers";
        const string expectedTopic = "testing/+/#";

        var mqttTopicFilter = new MqttTopicFilterBuilder()
            .WithTopicPattern(topicPattern)
            .WithQualityOfServiceLevel(qualityOfServiceLevel)
            .WithNoLocal(noLocal)
            .WithRetainAsPublished(retainAsPublished)
            .WithRetainHandling(retainHandling)
            .Build();

        Assert.NotNull(mqttTopicFilter);
        Assert.Equal(topicPattern, mqttTopicFilter.TopicPattern);
        Assert.Equal(expectedTopic, mqttTopicFilter.Topic);
        Assert.Equal(qualityOfServiceLevel, mqttTopicFilter.QualityOfServiceLevel);
        Assert.Equal(noLocal, mqttTopicFilter.NoLocal);
        Assert.Equal(retainAsPublished, mqttTopicFilter.RetainAsPublished);
        Assert.Equal(retainHandling, mqttTopicFilter.RetainHandling);
    }
}
