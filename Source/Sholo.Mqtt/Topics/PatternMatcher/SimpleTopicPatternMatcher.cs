using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sholo.Mqtt.Topics.FilterSanitizer;

namespace Sholo.Mqtt.Topics.PatternMatcher;

[PublicAPI]
internal class SimpleTopicPatternMatcher : ITopicPatternMatcher
{
    private static readonly IReadOnlyDictionary<string, string[]> EmptyDictionary = new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>());
    private static readonly IReadOnlySet<string> EmptySet = new HashSet<string>();

    public string Topic { get; }
    public string TopicPattern { get; }
    public IReadOnlySet<string> TopicParameterNames { get; }
    public string? MutliLevelWildcardParameterName { get; }

    public SimpleTopicPatternMatcher(string topicPattern)
    {
        Topic = TopicFilterSanitizer.SanitizeTopic(topicPattern);
        TopicPattern = topicPattern;
        TopicParameterNames = EmptySet;
    }

    public bool IsTopicMatch(string topic, out IReadOnlyDictionary<string, string[]>? topicArguments)
    {
        if (topic.Equals(TopicPattern, StringComparison.Ordinal))
        {
            topicArguments = EmptyDictionary;
            return true;
        }

        topicArguments = null;
        return false;
    }
}
