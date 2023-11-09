#nullable enable

using Sholo.Mqtt.Topics.PatternMatcher;

namespace Sholo.Mqtt.Topics.PatternMatcherFactory;

internal interface ITopicPatternMatcherFactory
{
    ITopicPatternMatcher CreateTopicPatternMatcher(string topicPattern);
}
