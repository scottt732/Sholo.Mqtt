using System.Collections.Generic;
using System.Text.RegularExpressions;
using MQTTnet;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.Topics.PatternFilter
{
    public class MqttTopicPatternFilter : MqttTopicFilter, IMqttTopicPatternFilter
    {
        public string TopicPattern { get; set; }
        public string[] TopicParameterNames { get; }

        private Regex TopicPatternMatcher { get; }

        public MqttTopicPatternFilter(
            string topic,
            string topicPattern,
            Regex topicPatternMatcher,
            MqttQualityOfServiceLevel? qualityOfServiceLevel,
            bool noLocal,
            bool retainAsPublished,
            MqttRetainHandling retainHandling,
            string[] topicParameterNames
        )
        {
            Topic = topic;
            TopicPattern = topicPattern;
            TopicPatternMatcher = topicPatternMatcher;
            TopicParameterNames = topicParameterNames;
            QualityOfServiceLevel = qualityOfServiceLevel ?? MqttQualityOfServiceLevel.AtMostOnce;
            NoLocal = noLocal;
            RetainAsPublished = retainAsPublished;
            RetainHandling = retainHandling;
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
}
