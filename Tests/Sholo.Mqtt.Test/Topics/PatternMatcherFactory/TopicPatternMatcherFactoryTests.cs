using System;
using Sholo.Mqtt.Topics.PatternMatcher;
using Sholo.Mqtt.Topics.PatternMatcherFactory;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.PatternMatcherFactory;

public class TopicPatternMatcherFactoryTests
{
    [Theory]
    [InlineData("testing")]
    [InlineData("testing/testing")]
    [InlineData("testing/testing/1/2/3")]
    [InlineData("is/this/thing/on")]
    public void CreateTopicPatternMatcher_WhenTopicPatternIsSimple_CreatesSimpleTopicPatternMatcher(string fixedTopicPattern)
    {
        var topicPatternMatcher = new TopicPatternMatcherFactory().CreateTopicPatternMatcher(fixedTopicPattern);

        Assert.NotNull(topicPatternMatcher);
        Assert.IsType<SimpleTopicPatternMatcher>(topicPatternMatcher);

        Assert.Equal(fixedTopicPattern, topicPatternMatcher.TopicPattern);
        Assert.Empty(topicPatternMatcher.TopicParameterNames);
        Assert.Null(topicPatternMatcher.MutliLevelWildcardParameterName);
    }

    [Theory]
    [InlineData("+test", "+", false)]
    [InlineData("testing/+test", "testing/+", false)]
    [InlineData("is/this/+device/on", "is/this/+/on", false)]
    [InlineData("is/this/+device/+state", "is/this/+/+", false)]
    [InlineData("#wildcard", "#", true)]
    [InlineData("testing/testing/#numbers", "testing/testing/#", true)]
    [InlineData("testing/+device/#numbers", "testing/+/#", true)]
    public void CreateTopicPatternMatcher_WhenTopicPatternIsComplex_CreatesComplexTopicPatternMatcher(string complexTopicPattern, string expectedTopic, bool hasWildcardParameter)
    {
        var topicPatternMatcher = new TopicPatternMatcherFactory().CreateTopicPatternMatcher(complexTopicPattern);

        Assert.NotNull(topicPatternMatcher);
        Assert.IsType<ComplexTopicPatternMatcher>(topicPatternMatcher);

        Assert.Equal(complexTopicPattern, topicPatternMatcher.TopicPattern);
        Assert.Equal(expectedTopic, topicPatternMatcher.Topic);
        Assert.NotEmpty(topicPatternMatcher.TopicParameterNames);

        if (hasWildcardParameter)
        {
            Assert.NotNull(topicPatternMatcher.MutliLevelWildcardParameterName);
        }
        else
        {
            Assert.Null(topicPatternMatcher.MutliLevelWildcardParameterName);
        }
    }

    [Theory]
    [InlineData("+")]
    [InlineData("testing/+")]
    [InlineData("is/this/+/on")]
    [InlineData("is/this/+/+state")]
    [InlineData("#")]
    [InlineData("testing/testing/#")]
    [InlineData("testing/+device/#")]
    [InlineData("testing/+/#wildcard")]
    public void CreateTopicPatternMatcher_WhenTopicPatternIsComplexAndHasMissingVariableNames_ThrowsArgumentException(string complexTopicPattern)
    {
        var ane = Assert.Throws<ArgumentException>(() => new TopicPatternMatcherFactory().CreateTopicPatternMatcher(complexTopicPattern));

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
    public void CreateTopicPatternMatcher_WhenWildcardVariableAppearsInMiddleSegment_ThrowsArgumentException(string complexTopicPattern)
    {
        var ae = Assert.Throws<ArgumentException>(() => new TopicPatternMatcherFactory().CreateTopicPatternMatcher(complexTopicPattern));

        Assert.StartsWith("Multi-level wildcards can only appear at the end of a topic pattern.", ae.Message, StringComparison.Ordinal);
        Assert.Equal("topicPattern", ae.ParamName, StringComparer.Ordinal);
    }

    [Fact]
    public void CreateTopicPatternMatcher_WhenTopicPatternIsNull_ThrowsArgumentNullException()
    {
        var ane = Assert.Throws<ArgumentNullException>(() => new TopicPatternMatcherFactory().CreateTopicPatternMatcher(null!));

        Assert.StartsWith(
            "topicPattern is required.",
            ane.Message,
            StringComparison.Ordinal
        );
        Assert.Equal("topicPattern", ane.ParamName, StringComparer.Ordinal);
    }

    [Fact]
    public void CreateTopicPatternMatcher_WhenTopicPatternIsEmpty_ThrowsArgumentException()
    {
        var ae = Assert.Throws<ArgumentException>(() => new TopicPatternMatcherFactory().CreateTopicPatternMatcher(string.Empty));

        Assert.StartsWith("topicPattern must be non-empty", ae.Message, StringComparison.Ordinal);
        Assert.Equal("topicPattern", ae.ParamName, StringComparer.Ordinal);
    }
}
