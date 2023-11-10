using System;
using Sholo.Mqtt.Topics.PatternMatcher;
using Sholo.Mqtt.Topics.PatternMatcherFactory;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.PatternMatcher;

public class ComplexTopicPatternMatcherTests
{
    [Fact]
    public void IsTopicMatch_WhenTopicMatchesPattern_ReturnsTrueWithExpectedTopicArguments()
    {
        var topicPatternMatcher = new TopicPatternMatcherFactory().CreateTopicPatternMatcher("testing/+action/#numbers");
        var complexTopicPatternMatcher = Assert.IsType<ComplexTopicPatternMatcher>(topicPatternMatcher);

        Assert.Equal("testing/+/#", complexTopicPatternMatcher.Topic);
        Assert.Equal("testing/+action/#numbers", complexTopicPatternMatcher.TopicPattern);

        Assert.Collection(
            complexTopicPatternMatcher.TopicParameterNames,
            p => Assert.Equal("action", p, StringComparer.Ordinal),
            p => Assert.Equal("numbers", p, StringComparer.Ordinal)
        );

        Assert.NotNull(complexTopicPatternMatcher.MutliLevelWildcardParameterName);
        Assert.Equal("numbers", complexTopicPatternMatcher.MutliLevelWildcardParameterName);

        var result = complexTopicPatternMatcher.IsTopicMatch("testing/testing/1/2/3", out var topicArguments);

        Assert.True(result);

        Assert.NotNull(topicArguments);
        Assert.Collection(
            topicArguments,
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
    }

    [Fact]
    public void IsTopicMatch_WhenTopicDoesNotMatchPattern_ReturnsFalseWithNullTopicArguments()
    {
        var topicPatternMatcher = new TopicPatternMatcherFactory().CreateTopicPatternMatcher("testing/+action/#numbers");
        var complexTopicPatternMatcher = Assert.IsType<ComplexTopicPatternMatcher>(topicPatternMatcher);

        Assert.Equal("testing/+/#", complexTopicPatternMatcher.Topic);
        Assert.Equal("testing/+action/#numbers", complexTopicPatternMatcher.TopicPattern);

        Assert.Collection(
            complexTopicPatternMatcher.TopicParameterNames,
            p => Assert.Equal("action", p, StringComparer.Ordinal),
            p => Assert.Equal("numbers", p, StringComparer.Ordinal)
        );

        Assert.NotNull(complexTopicPatternMatcher.MutliLevelWildcardParameterName);
        Assert.Equal("numbers", complexTopicPatternMatcher.MutliLevelWildcardParameterName);

        var result = complexTopicPatternMatcher.IsTopicMatch("unmatched/topic/a/b/c", out var topicArguments);

        Assert.False(result);
        Assert.Null(topicArguments);
    }
}
