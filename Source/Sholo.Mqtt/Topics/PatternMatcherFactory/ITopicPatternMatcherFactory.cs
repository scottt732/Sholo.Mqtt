using Sholo.Mqtt.Topics.PatternMatcher;

namespace Sholo.Mqtt.Topics.PatternMatcherFactory;

[PublicAPI]
internal interface ITopicPatternMatcherFactory
{
    ITopicPatternMatcher CreateTopicPatternMatcher(string topicPattern);
}
