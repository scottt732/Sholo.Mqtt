using System;
using Sholo.Mqtt.Topic;
using Xunit;
using Xunit.Abstractions;

namespace Sholo.Mqtt.Test.Topics
{
    public class TopicParserTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TopicParserTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("something/#/somethingElse/#")]
        [InlineData("something/#/#/somethingElse")]
        [InlineData("sensor/data/foo/#/#")]
        [InlineData("#/data/foo/#")]
        [InlineData("#/a")]
        [InlineData("a/a/#/a/#")]
        public void TopicParser_WithMultipleMultiLevelWildcards_ThrowsArgumentException(string pattern)
        {
            var tp = new TopicParser();

            Assert.Throws<ArgumentException>(() => tp.ToMatchingRegex(pattern));
        }

        [Theory]
        [InlineData("something/+/somethingElse", "something/abc/somethingElse", true)]
        [InlineData("something/+/somethingElse", "something/abc/defg/somethingElse", false)]
        [InlineData("sensor/data/foo/#", "sensor/data/foo/abc/a13re/adff", true)]
        [InlineData("sensor/data/foo/#", "sensor/data/foo/abc", true)]
        [InlineData("sensor/+/bar/#", "sensor/foo/bar/baz/wibble/json", true)]
        [InlineData("sensor/+/bar/#", "sensor/bar/bar/black/sheep", true)]
        [InlineData("sensor/+/bar/#", "sensor/bar/abc/d", false)]

        public void TopicParser_WithSingleLevelWildcards_MatchesExpectedExamples(string pattern, string sample, bool expectedMatch)
        {
            var tp = new TopicParser();
            var reg = tp.ToMatchingRegex(pattern);

            _testOutputHelper.WriteLine(pattern);
            _testOutputHelper.WriteLine(reg.ToString());
            _testOutputHelper.WriteLine(sample);

            var match = reg.Match(sample);

            Assert.Equal(expectedMatch, match.Success);
        }
    }
}
