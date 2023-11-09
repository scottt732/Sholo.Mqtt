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

    [Fact]
    public void CreateTopicPatternMatcher_WhenTopicPatternIsComplex_CreatesComplexTopicPatternMatcher()
    {
    }
}
