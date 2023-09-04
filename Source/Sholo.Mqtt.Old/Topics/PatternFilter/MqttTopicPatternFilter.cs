using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using MQTTnet;
using MQTTnet.Protocol;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfiguration;

namespace Sholo.Mqtt.Old.Topics.PatternFilter
{
    public class MqttTopicPatternFilter : MqttTopicFilter, IMqttTopicPatternFilter
    {
        public string TopicPattern { get; set; }

        private Regex RegexPattern { get; }
        private IList<string> RegisteredVariables { get; }
        private IDictionary<string, IMqttTopicPatternPropertyConfiguration> PropertyConfigurations { get; }

        public MqttTopicPatternFilter(
            string topic,
            string topicPattern,
            Regex regexPattern,
            IList<string> registeredVariables,
            IDictionary<string, IMqttTopicPatternPropertyConfiguration> propertyConfigurations,
            MqttQualityOfServiceLevel? qualityOfServiceLevel,
            bool? noLocal,
            bool? retainAsPublished,
            MqttRetainHandling? retainHandling
        )
        {
            Topic = topic;
            TopicPattern = topicPattern;
            RegexPattern = regexPattern;
            RegisteredVariables = registeredVariables;
            PropertyConfigurations = propertyConfigurations;
            QualityOfServiceLevel = qualityOfServiceLevel ?? MqttQualityOfServiceLevel.AtMostOnce;
            NoLocal = noLocal;
            RetainAsPublished = retainAsPublished;
            RetainHandling = retainHandling;
        }

        public bool IsMatch(string topic) => RegexPattern.IsMatch(topic);

        public void Bind(string topic, MethodInfo target)
        {
            var match = RegexPattern.Match(topic);

            if (!match.Success)
            {
                throw new ArgumentException("The supplied topic did not match the configured pattern");
            }

            var parameters = target.GetParameters();

            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType.IsByRef)
                {
                    if (parameter.HasDefaultValue)
                    {
                    }
                }
                else
                {
                    // TODO: Handle Nullable<int> type values
                }
            }

            /*
            var target = TopicParametersFactory.Invoke();
            foreach (var registeredVariable in RegisteredVariables)
            {
                var value = match.Groups[registeredVariable].Value;
                var setter = PropertySetters[registeredVariable];

                setter.SetValue(target, value);
            }

            return target;
            */
        }
    }
}
