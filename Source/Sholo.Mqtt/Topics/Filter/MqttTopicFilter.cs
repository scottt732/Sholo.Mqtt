using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.PatternMatcher;

namespace Sholo.Mqtt.Topics.Filter;

public class MqttTopicFilter : IMqttTopicFilter
{
    public string Topic => TopicPatternMatcher.Topic;
    public string TopicPattern => TopicPatternMatcher.TopicPattern;
    public MqttQualityOfServiceLevel QualityOfServiceLevel { get; }
    public bool NoLocal { get; }
    public bool RetainAsPublished { get; }
    public MqttRetainHandling RetainHandling { get; }

    private ITopicPatternMatcher TopicPatternMatcher { get; }

    public bool IsMatch(IMqttRequestContext context, [MaybeNullWhen(false)] out IReadOnlyDictionary<string, StringValues> topicArguments)
    {
        topicArguments = null;
        return context.QualityOfServiceLevel == QualityOfServiceLevel && TopicPatternMatcher.IsTopicMatch(context.Topic, out topicArguments);
    }

    public MqttTopicFilter(ITopicPatternMatcher topicPatternMatcher, MqttQualityOfServiceLevel qualityOfServiceLevel, bool noLocal, bool retainAsPublished, MqttRetainHandling retainHandling)
    {
        TopicPatternMatcher = topicPatternMatcher;
        QualityOfServiceLevel = qualityOfServiceLevel;
        RetainHandling = retainHandling;
        RetainAsPublished = retainAsPublished;
        NoLocal = noLocal;
    }

    public override string ToString() => $"TopicFilter: [Topic={Topic}] [QualityOfServiceLevel={QualityOfServiceLevel}] [NoLocal={NoLocal}] [RetainAsPublished={RetainAsPublished}] [RetainHandling={RetainHandling}]";
}
