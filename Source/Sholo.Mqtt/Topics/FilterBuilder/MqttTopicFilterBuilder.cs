using System.Reflection;
using MQTTnet.Protocol;
using Sholo.Mqtt.Controllers;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.PatternMatcherFactory;

namespace Sholo.Mqtt.Topics.FilterBuilder;

internal class MqttTopicFilterBuilder : IMqttTopicFilterBuilder
{
    private string? TopicPattern { get; set; }
    private bool CaseSensitive { get; set; } = true;
    private MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }
    private bool? NoLocal { get; set; }
    private bool? RetainAsPublished { get; set; }
    private MqttRetainHandling? RetainHandling { get; set; }

    public static IMqttTopicFilterBuilder FromActionAttributes(TypeInfo? instance, MethodInfo action)
    {
        var topicPrefixAttribute = instance?.GetCustomAttribute<TopicPrefixAttribute>();

        var topicAttribute = action.GetCustomAttribute<TopicAttribute>() ??
                             instance?.GetCustomAttribute<TopicAttribute>();

        var noLocalAttribute = action.GetCustomAttribute<NoLocalAttribute>() ??
                               instance?.GetCustomAttribute<NoLocalAttribute>();

        var qualityOfServiceAttribute = action.GetCustomAttribute<QualityOfServiceAttribute>() ??
                                        instance?.GetCustomAttribute<QualityOfServiceAttribute>();

        var retainAsPublishedAttribute = action.GetCustomAttribute<RetainAsPublishedAttribute>() ??
                                         instance?.GetCustomAttribute<RetainAsPublishedAttribute>();

        var retainHandlingAttribute = action.GetCustomAttribute<RetainHandlingAttribute>() ??
                                      instance?.GetCustomAttribute<RetainHandlingAttribute>();

        var mqttTopicFilterBuilder = new MqttTopicFilterBuilder();

        if (noLocalAttribute?.NoLocal != null)
        {
            mqttTopicFilterBuilder.WithNoLocal(noLocalAttribute.NoLocal);
        }

        if (qualityOfServiceAttribute?.QualityOfServiceLevel != null)
        {
            mqttTopicFilterBuilder.WithQualityOfServiceLevel(qualityOfServiceAttribute.QualityOfServiceLevel);
        }

        if (retainAsPublishedAttribute?.RetainAsPublished != null)
        {
            mqttTopicFilterBuilder.WithRetainAsPublished(retainAsPublishedAttribute.RetainAsPublished);
        }

        if (retainHandlingAttribute?.RetainHandling != null)
        {
            mqttTopicFilterBuilder.WithRetainHandling(retainHandlingAttribute.RetainHandling);
        }

        var topicPrefix = topicPrefixAttribute?.TopicPrefix.TrimEnd('/');
        var topicPattern = topicAttribute?.TopicPattern.TrimStart('/');
        var effectiveTopicPattern = !string.IsNullOrEmpty(topicPrefix)
            ? $"{topicPrefix}/{topicPattern}"
            : topicPattern;

        if (effectiveTopicPattern != null)
        {
            // TODO: Case sensitive -> configurable
            mqttTopicFilterBuilder.WithTopicPattern(effectiveTopicPattern);
        }

        return mqttTopicFilterBuilder;
    }

    public IMqttTopicFilterBuilder WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel)
    {
        QualityOfServiceLevel = qualityOfServiceLevel;
        return this;
    }

    public IMqttTopicFilterBuilder WithTopicPattern(string topicPattern)
    {
        TopicPattern = topicPattern;
        CaseSensitive = true; // TODO
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
            .CreateTopicPatternMatcher(TopicPattern!); // , CaseSensitive

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
