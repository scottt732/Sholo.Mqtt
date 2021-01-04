using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sholo.Mqtt.Topics.Binder
{
    internal class TopicBinder<TTargetModel> : ITopicBinder<TTargetModel>
    {
        public string MqttPattern { get; }

        private Func<TTargetModel> TargetFactory { get; }
        private Regex RegexPattern { get; }
        private IList<string> RegisteredVariables { get; }
        private IDictionary<string, Action<TTargetModel, string>> PropertySetters { get; }

        public TopicBinder(
            Func<TTargetModel> targetFactory,
            IDictionary<string, Action<TTargetModel, string>> propertySetters,
            Regex regexPattern,
            IList<string> registeredVariables,
            string mqttPattern)
        {
            TargetFactory = targetFactory;
            MqttPattern = mqttPattern;
            RegexPattern = regexPattern;
            RegisteredVariables = registeredVariables;
            PropertySetters = propertySetters;
        }

        public bool IsMatch(string topic) => RegexPattern.IsMatch(topic);

        public TTargetModel Bind(string topic)
        {
            if (RegisteredVariables.Count == 0)
            {
                return TargetFactory();
            }

            var match = RegexPattern.Match(topic);

            if (!match.Success)
            {
                throw new ArgumentException("The supplied topic did not match the configured pattern");
            }

            var target = TargetFactory.Invoke();
            foreach (var registeredVariable in RegisteredVariables)
            {
                var value = match.Groups[registeredVariable].Value;
                var setter = PropertySetters[registeredVariable];

                setter.Invoke(target, value);
            }

            return target;
        }
    }
}
