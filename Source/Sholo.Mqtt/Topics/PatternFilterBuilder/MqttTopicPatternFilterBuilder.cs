using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MQTTnet.Protocol;
using Sholo.Mqtt.Topics.FilterSanitizer;
using Sholo.Mqtt.Topics.PatternFilter;
using Sholo.Mqtt.Topics.PatternPropertyConfiguration;

namespace Sholo.Mqtt.Topics.PatternFilterBuilder
{
    // Removing in favor of adding a `Bind<TTopicParameters>()` extension method to IMqttTopicFilter,
    // TTopicParameters' property configuration methods to DI layer (IMqttServiceCollection).
    // This keeps IMqttApplicationBuilder simpler

    internal class MqttTopicPatternFilterBuilder : IMqttTopicPatternFilterBuilder
    {
        private string Topic { get; set; }
        private string TopicPattern { get; set; }
        private MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }
        private bool NoLocal { get; set; }
        private bool RetainAsPublished { get; set; }
        private MqttRetainHandling RetainHandling { get; set; }

        public IMqttTopicPatternFilterBuilder WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel)
        {
            QualityOfServiceLevel = qualityOfServiceLevel;
            return this;
        }

        public IMqttTopicPatternFilterBuilder WithTopicPattern(string topicPattern)
        {
            Topic = TopicFilterSanitizer.SanitizeTopic(topicPattern);
            TopicPattern = topicPattern;
            return this;
        }

        public IMqttTopicPatternFilterBuilder WithNoLocal(bool noLocal)
        {
            NoLocal = noLocal;
            return this;
        }

        public IMqttTopicPatternFilterBuilder WithRetainAsPublished(bool retainAsPublished)
        {
            RetainAsPublished = retainAsPublished;
            return this;
        }

        public IMqttTopicPatternFilterBuilder WithRetainHandling(MqttRetainHandling retainHandling)
        {
            RetainHandling = retainHandling;
            return this;
        }

        public IMqttTopicPatternFilter Build()
        {
            if (Topic == null)
            {
                throw new ArgumentNullException(nameof(Topic), $"The {nameof(Topic)} cannot be null");
            }

            if (Topic.Length == 0)
            {
                throw new ArgumentException($"The {nameof(Topic)} must be non-empty", nameof(Topic));
            }

            var topicParts = TopicPattern.Split('/');
            var regBuilder = new StringBuilder('^');

            var haveMultiLevelWildcard = false;

            var topicParameterNames = new List<string>();
            for (var i = 0; i < topicParts.Length; i++)
            {
                var topicPart = topicParts[i];
                if (topicPart.StartsWith("+", StringComparison.Ordinal))
                {
                    var variableName = topicPart.Substring(1);
                    topicParameterNames.Add(variableName);

                    regBuilder.Append("(?<" + variableName + ">[^/]+)");
                }
                else if (topicPart.StartsWith("#", StringComparison.Ordinal))
                {
                    if (i != topicParts.Length - 1)
                    {
                        throw new ArgumentException("Multi-level wildcards can only appear at the end of a topic pattern");
                    }

                    if (haveMultiLevelWildcard)
                    {
                        throw new ArgumentException("Only one multi-level wildcard can appear within a topic pattern");
                    }

                    var variableName = topicPart.Substring(1);
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

            var result = new MqttTopicPatternFilter(
                Topic,
                TopicPattern,
                regex,
                QualityOfServiceLevel,
                NoLocal,
                RetainAsPublished,
                RetainHandling,
                topicParameterNames.ToArray());

            return result;
        }

        private void ValidatePropertiesHaveSetters(
            IList<string> registeredVariables,
            IDictionary<string, IMqttTopicPatternPropertyConfiguration> propertyConfigurationsByTopicParameterName)
        {
            StringBuilder sb = null;
            foreach (var unconfiguredVariable in registeredVariables.Where(x => !propertyConfigurationsByTopicParameterName.ContainsKey(x)))
            {
                if (sb == null)
                {
                    sb = new StringBuilder();
                }

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
    }
 }
