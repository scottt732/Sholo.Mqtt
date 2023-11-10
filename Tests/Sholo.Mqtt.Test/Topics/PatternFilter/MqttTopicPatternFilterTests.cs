namespace Sholo.Mqtt.Test.Topics.PatternFilter;

public class MqttTopicPatternFilterTests
{
    /*
        [Fact]
        public void IsMatch_WhenSingleLevelWildcardMatches_ReturnsTrueAndReturnsExpectedDictionary()
        {
            var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
                .WithTopicPattern("user/+username")
                .Build();

            var matchResult = mqttTopicPatternFilter.IsMatch("user/sholodak", out var topicArguments);

            Assert.True(matchResult);
            Assert.NotNull(topicArguments);
            Assert.Collection(
                topicArguments,
                kvp =>
                {
                    Assert.Equal("username", kvp.Key);
                    Assert.Collection(
                        kvp.Value,
                        v => Assert.Equal("sholodak", v, StringComparer.Ordinal));
                });
        }

        [Fact]
        public void IsMatch_WhenMultiLevelWildcardMatches_ReturnsTrueAndReturnsExpectedDictionary()
        {
            var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
                .WithTopicPattern("users/#usernames")
                .Build();

            var matchResult = mqttTopicPatternFilter.IsMatch("users/user1/user2/user3", out var topicArguments);

            Assert.True(matchResult);
            Assert.NotNull(topicArguments);
            Assert.Collection(
                topicArguments,
                kvp =>
                {
                    Assert.Equal("usernames", kvp.Key);
                    Assert.Collection(
                        kvp.Value,
                        u => Assert.Equal("user1", u, StringComparer.Ordinal),
                        u => Assert.Equal("user2", u, StringComparer.Ordinal),
                        u => Assert.Equal("user3", u, StringComparer.Ordinal)
                    );
                }
            );
        }

        [Fact]
        public void IsMatch_WhenMultipleTypesOfWildcardsMatch_ReturnsTrueAndReturnsExpectedDictionary()
        {
            var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
                .WithTopicPattern("book/+title/+chapter/#section")
                .Build();

            var matchResult = mqttTopicPatternFilter.IsMatch("book/My Book/Chapter 1: Getting Started/1/2/3/4", out var topicArguments);

            Assert.True(matchResult);
            Assert.NotNull(topicArguments);
            Assert.Collection(
                topicArguments,
                kvp =>
                {
                    Assert.Equal("title", kvp.Key);
                    Assert.Collection(
                        kvp.Value,
                        b => Assert.Equal("My Book", b, StringComparer.Ordinal)
                    );
                },
                kvp =>
                {
                    Assert.Equal("chapter", kvp.Key);
                    Assert.Collection(
                        kvp.Value,
                        c => Assert.Equal("Chapter 1: Getting Started", c, StringComparer.Ordinal)
                    );
                },
                kvp =>
                {
                    Assert.Equal("section", kvp.Key);
                    Assert.Collection(
                        kvp.Value,
                        s => Assert.Equal("1", s, StringComparer.Ordinal),
                        s => Assert.Equal("2", s, StringComparer.Ordinal),
                        s => Assert.Equal("3", s, StringComparer.Ordinal),
                        s => Assert.Equal("4", s, StringComparer.Ordinal)
                    );
                }
            );
        }

        [Fact]
        public void IsMatch_WhenTopicDoesntMatch_ReturnsFalseWithNullTopicArguments()
        {
            var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
                .WithTopicPattern("user/+username")
                .Build();

            var matchResult = mqttTopicPatternFilter.IsMatch("different/topic", out var topicArguments);

            Assert.False(matchResult);
            Assert.Null(topicArguments);
        }
    }
    */
}
