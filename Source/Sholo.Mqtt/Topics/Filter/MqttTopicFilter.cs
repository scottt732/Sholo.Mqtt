using System.Collections.Generic;
using MQTTnet.Protocol;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.Topics.PatternMatcher;

namespace Sholo.Mqtt.Topics.Filter;

public class MqttTopicFilter : IMqttTopicFilter
{
    public string Topic => TopicPatternMatcher.Topic;
    public MqttQualityOfServiceLevel QualityOfServiceLevel { get; }
    public bool NoLocal { get; }
    public bool RetainAsPublished { get; }
    public MqttRetainHandling RetainHandling { get; }

    private ITopicPatternMatcher TopicPatternMatcher { get; }

    public bool IsMatch(IMqttRequestContext context, out IReadOnlyDictionary<string, string[]>? topicArguments) => TopicPatternMatcher.IsTopicMatch(context.Topic, out topicArguments);

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
