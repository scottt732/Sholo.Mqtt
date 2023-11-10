using Sholo.Mqtt.Topics.PatternMatcher;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.PatternMatcher;

public class SimpleTopicPatternMatcherTests
{
    [Fact]
    public void IsMatch_WhenStringsMatch_ReturnsTrueWithEmptyTopicArguments()
    {
        var simpleTopicPatternMatcher = new SimpleTopicPatternMatcher("testing/testing/1/2/3");

        var result = simpleTopicPatternMatcher.IsTopicMatch("testing/testing/1/2/3", out var topicArguments);

        Assert.True(result);
        Assert.NotNull(topicArguments);
        Assert.Empty(topicArguments);
    }

    [Fact]
    public void IsMatch_WhenStringsDontMatch_ReturnsFalseWithNullTopicArguments()
    {
        var simpleTopicPatternMatcher = new SimpleTopicPatternMatcher("testing/testing/1/2/3");

        var result = simpleTopicPatternMatcher.IsTopicMatch("testing/testing/4/5/6", out var topicArguments);

        Assert.False(result);
        Assert.Null(topicArguments);
    }
}
