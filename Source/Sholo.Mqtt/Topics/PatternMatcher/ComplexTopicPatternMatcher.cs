using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Sholo.Mqtt.Topics.FilterSanitizer;

namespace Sholo.Mqtt.Topics.PatternMatcher;

[PublicAPI]
internal class ComplexTopicPatternMatcher : ITopicPatternMatcher
{
    public string Topic { get; set; }
    public string TopicPattern { get; }
    public IReadOnlySet<string> TopicParameterNames { get; }
    public string? MutliLevelWildcardParameterName { get; }

    private Regex? PatternMatcher { get; }

    public bool IsTopicMatch(string topic, out IReadOnlyDictionary<string, string[]>? topicArguments)
    {
        var match = PatternMatcher!.Match(topic);
        if (!match.Success)
        {
            topicArguments = null;
            return false;
        }

        var result = new Dictionary<string, List<string>>();

        foreach (var topicParameterName in TopicParameterNames)
        {
            var matchGroup = match.Groups[topicParameterName];
            if (matchGroup.Success)
            {
                if (!result.TryGetValue(topicParameterName, out var topicArgumentsList))
                {
                    topicArgumentsList = new List<string>();
                    result[topicParameterName] = topicArgumentsList;
                }

                if (topicParameterName == MutliLevelWildcardParameterName)
                {
                    topicArgumentsList.AddRange(matchGroup.Value.Split('/'));
                }
                else
                {
                    topicArgumentsList.Add(matchGroup.Value);
                }
            }
        }

        topicArguments = new ReadOnlyDictionary<string, string[]>(result.ToDictionary(x => x.Key, x => x.Value.ToArray()));
        return true;
    }

    public ComplexTopicPatternMatcher(
        string topicPattern,
        Regex patternMatcher,
        IReadOnlySet<string> topicParameterNames,
        string? mutliLevelWildcardParameterName
    )
    {
        Topic = TopicFilterSanitizer.SanitizeTopic(topicPattern);
        TopicPattern = topicPattern;
        PatternMatcher = patternMatcher;
        TopicParameterNames = topicParameterNames;
        MutliLevelWildcardParameterName = mutliLevelWildcardParameterName;
    }
}
