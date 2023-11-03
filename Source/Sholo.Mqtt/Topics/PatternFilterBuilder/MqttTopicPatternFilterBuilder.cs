using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.FilterBuilder;
using Sholo.Mqtt.Topics.FilterSanitizer;
using Sholo.Mqtt.Topics.PatternFilter;

namespace Sholo.Mqtt.Topics.PatternFilterBuilder;

// Removing in favor of adding a `Bind<TTopicParameters>()` extension method to IMqttTopicFilter,
// TTopicParameters' property configuration methods to DI layer (IMqttServiceCollection).
// This keeps IMqttApplicationBuilder simpler

internal class MqttTopicPatternFilterBuilder : IMqttTopicPatternFilterBuilder
{
    private string TopicPattern { get; set; }
    private MqttTopicFilterBuilder TopicFilterBuilder { get; } = new MqttTopicFilterBuilder();

    public IMqttTopicPatternFilterBuilder WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel)
    {
        TopicFilterBuilder.WithQualityOfServiceLevel(qualityOfServiceLevel);
        return this;
    }

    public IMqttTopicPatternFilterBuilder WithTopicPattern(string topicPattern)
    {
        TopicFilterBuilder.WithTopic(TopicFilterSanitizer.SanitizeTopic(topicPattern));
        TopicPattern = topicPattern;
        return this;
    }

    public IMqttTopicPatternFilterBuilder WithNoLocal(bool noLocal)
    {
        TopicFilterBuilder.WithNoLocal(noLocal);
        return this;
    }

    public IMqttTopicPatternFilterBuilder WithRetainAsPublished(bool retainAsPublished)
    {
        TopicFilterBuilder.WithRetainAsPublished(retainAsPublished);
        return this;
    }

    public IMqttTopicPatternFilterBuilder WithRetainHandling(MqttRetainHandling retainHandling)
    {
        TopicFilterBuilder.WithRetainHandling(retainHandling);
        return this;
    }

    public IMqttTopicPatternFilter Build()
    {
        var topicParts = TopicPattern.Split('/');
        var regBuilder = new StringBuilder("^");

        var haveMultiLevelWildcard = false;

        var topicParameterNames = new List<string>();
        for (var i = 0; i < topicParts.Length; i++)
        {
            var topicPart = topicParts[i];
            if (topicPart.StartsWith('+'))
            {
                var variableName = topicPart[1..];
                topicParameterNames.Add(variableName);

                regBuilder.Append("(?<" + variableName + ">[^/]+)");
            }
            else if (topicPart.StartsWith('#'))
            {
                if (i != topicParts.Length - 1)
                {
                    throw new ArgumentException("Multi-level wildcards can only appear at the end of a topic pattern");
                }

                if (haveMultiLevelWildcard)
                {
                    throw new ArgumentException("Only one multi-level wildcard can appear within a topic pattern");
                }

                var variableName = topicPart[1..];
                topicParameterNames.Add(variableName);

                regBuilder.Append("(?<" + variableName + ">([^/]+/)*([^/]+))");
                haveMultiLevelWildcard = true;
            }
            else
            {
                regBuilder.Append(topicPart);
            }

            regBuilder.Append('/');
        }

        if (regBuilder.Length > 0)
        {
            regBuilder.Length -= 1;
        }

        regBuilder.Append('$');

        var regex = new Regex(regBuilder.ToString());

        var topicFilter = TopicFilterBuilder.Build();

        var result = new MqttTopicPatternFilter(
            topicFilter,
            TopicPattern,
            regex,
            topicParameterNames.ToArray());

        return result;
    }

    /*
    private void ValidatePropertiesHaveSetters(
        IList<string> registeredVariables,
        IDictionary<string, IMqttTopicPatternPropertyConfiguration> propertyConfigurationsByTopicParameterName)
    {
        StringBuilder sb = null;
        foreach (var unconfiguredVariable in registeredVariables.Where(x => !propertyConfigurationsByTopicParameterName.ContainsKey(x)))
        {
            sb ??= new StringBuilder();
            sb.Append(unconfiguredVariable + ", ");
        }

        if (sb != null)
        {
            sb.Length -= 2;
            throw new ArgumentException("The following properties do not have setters or were not configured: " + sb);
        }
    }

    private void ValidatePropertiesHaveTypeConverters(
        IList<string> registeredVariables,
        IDictionary<string, IMqttTopicPatternPropertyConfiguration> propertyConfigurationsByTopicParameterName)
    {
        var requiredPropertiesWithoutTypeConverters = propertyConfigurationsByTopicParameterName
            .Where(x => registeredVariables.Contains(x.Key))
            .Where(x => !x.Value.HaveTypeConverter)
            .Select(x => x.Value.ParameterName)
            .ToArray();

        if (requiredPropertiesWithoutTypeConverters.Length > 0)
        {
            throw new ArgumentException(
                "The following properties do not have type converters configured: " +
                string.Join(", ", requiredPropertiesWithoutTypeConverters));
        }
    }
    */
}
