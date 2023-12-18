using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Primitives;
using Sholo.Mqtt.Topics.FilterSanitizer;
using Sholo.Mqtt.Utilities;

namespace Sholo.Mqtt.Topics.PatternMatcher;

[PublicAPI]
internal class SimpleTopicPatternMatcher : ITopicPatternMatcher
{
    private static readonly IReadOnlyDictionary<string, StringValues> EmptyDictionary = ReadOnlyDictionary<string, StringValues>.Empty;
    private static readonly IReadOnlySet<string> EmptySet = ReadOnlySet.Empty<string>();

    public string Topic { get; }
    public string TopicPattern { get; }
    public IReadOnlySet<string> TopicParameterNames { get; }
    public string? MutliLevelWildcardParameterName { get; }

    private StringComparison StringComparison { get; }

    public SimpleTopicPatternMatcher(string topicPattern) // TODO: , bool caseSensitive
    {
        Topic = TopicFilterSanitizer.SanitizeTopic(topicPattern);
        TopicPattern = topicPattern;
        StringComparison = StringComparison.Ordinal; // TODO: configurable
        TopicParameterNames = EmptySet;
    }

    public bool IsTopicMatch(string topic, out IReadOnlyDictionary<string, StringValues>? topicArguments)
    {
        if (topic.Equals(TopicPattern, StringComparison))
        {
            topicArguments = EmptyDictionary;
            return true;
        }

        topicArguments = null;
        return false;
    }
}
