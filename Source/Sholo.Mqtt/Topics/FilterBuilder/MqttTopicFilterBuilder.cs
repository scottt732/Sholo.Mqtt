using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.PatternMatcherFactory;

namespace Sholo.Mqtt.Topics.FilterBuilder;

internal class MqttTopicFilterBuilder : IMqttTopicFilterBuilder
{
    private string? TopicPattern { get; set; }
    private MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }
    private bool? NoLocal { get; set; }
    private bool? RetainAsPublished { get; set; }
    private MqttRetainHandling? RetainHandling { get; set; }

    public IMqttTopicFilterBuilder WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel)
    {
        QualityOfServiceLevel = qualityOfServiceLevel;
        return this;
    }

    public IMqttTopicFilterBuilder WithTopicPattern(string topicPattern)
    {
        TopicPattern = topicPattern;
        return this;
    }

    public IMqttTopicFilterBuilder WithNoLocal(bool noLocal)
    {
        NoLocal = noLocal;
        return this;
    }

    public IMqttTopicFilterBuilder WithRetainAsPublished(bool retainAsPublished)
    {
        RetainAsPublished = retainAsPublished;
        return this;
    }

    public IMqttTopicFilterBuilder WithRetainHandling(MqttRetainHandling retainHandling)
    {
        RetainHandling = retainHandling;
        return this;
    }

    public IMqttTopicFilter Build()
    {
        var topicPatternMatcher = new TopicPatternMatcherFactory()
            .CreateTopicPatternMatcher(TopicPattern!);

        var result = new MqttTopicFilter(
            topicPatternMatcher,
            QualityOfServiceLevel ?? MqttQualityOfServiceLevel.AtMostOnce,
            NoLocal ?? false,
            RetainAsPublished ?? false,
            RetainHandling ?? MqttRetainHandling.SendAtSubscribe
        );

        return result;
    }
}
