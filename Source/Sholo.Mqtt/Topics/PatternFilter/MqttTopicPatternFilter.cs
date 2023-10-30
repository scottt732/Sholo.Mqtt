using System.Collections.Generic;
using System.Text.RegularExpressions;
using MQTTnet.Packets;

namespace Sholo.Mqtt.Topics.PatternFilter;

public class MqttTopicPatternFilter : IMqttTopicPatternFilter
{
    public string TopicPattern { get; set; }
    public string[] TopicParameterNames { get; }

    public MqttTopicFilter TopicFilter { get; }

    private Regex TopicPatternMatcher { get; }

    public MqttTopicPatternFilter(
        MqttTopicFilter topicFilter,
        string topicPattern,
        Regex topicPatternMatcher,
        string[] topicParameterNames
    )
    {
        TopicFilter = topicFilter;
        TopicPattern = topicPattern;
        TopicPatternMatcher = topicPatternMatcher;
        TopicParameterNames = topicParameterNames;
    }

    public bool IsMatch(string topic, out IDictionary<string, string> topicArguments)
    {
        var match = TopicPatternMatcher.Match(topic);
        if (!match.Success)
        {
            topicArguments = null;
            return false;
        }

        topicArguments = new Dictionary<string, string>();

        foreach (var topicParameterName in TopicParameterNames)
        {
            var matchGroup = match.Groups[topicParameterName];
            if (matchGroup.Success)
            {
                topicArguments[topicParameterName] = matchGroup.Value;
            }
        }

        return true;
    }
}
