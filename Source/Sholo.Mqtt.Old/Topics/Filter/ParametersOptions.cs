using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfiguration;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfigurationBuilder;
using Sholo.Mqtt.Old.Utilities;

namespace Sholo.Mqtt.Old.Topics.Filter
{
    public class ParametersOptions<TTopicParameters> : IParametersOptions<TTopicParameters>
        where TTopicParameters : class
    {
        private Func<TTopicParameters> TopicParametersFactory { get; set; }
        private IDictionary<string, List<Action<IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>>>> PropertyConfigurations { get; } = new Dictionary<string, List<Action<IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>>>>();

        public IParametersOptions<TTopicParameters> WithFactory(Func<TTopicParameters> topicParametersFactory)
        {
            this.TopicParametersFactory = topicParametersFactory;
            return this;
        }

        public IParametersOptions<TTopicParameters> ForProperty<TProperty>(
            Expression<Func<TTopicParameters, TProperty>> propertySelector,
            Action<IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>> configuration)
        {
            if (propertySelector.Body is not MemberExpression memberExpression
                || memberExpression.Member is not PropertyInfo property)
            {
                throw new ArgumentException(
                    $"The {nameof(propertySelector)} must select a public property on {typeof(TTopicParameters).Name}",
                    nameof(propertySelector));
            }

            var propertyName = property.Name;

            if (!PropertyConfigurations.TryGetValue(propertyName, out var configurations))
            {
                configurations = new List<Action<IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>>>();
                PropertyConfigurations[propertyName] = configurations;
            }

            configurations.Add(configuration);

            return this;
        }

        public TTopicParameters CreateInstance()
        {
            return TopicParametersFactory?.Invoke()
                   ?? Activator.CreateInstance<TTopicParameters>();
        }

        public void BindParameters(
            TTopicParameters instance,
            IDictionary<string, IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>> propertyConfigurationBuilders,
            IDictionary<string, string> variableStringValues)
        {
            var propertyConfigurations = Configure(propertyConfigurationBuilders);

            foreach (var entry in variableStringValues)
            {
                var variableName = entry.Key;
                var stringValue = entry.Value;

                if (!propertyConfigurations.TryGetValue(stringValue, out var configuration))
                {
                    AttemptToConvertAndSetUnconfiguredProperty(instance, variableName, stringValue);
                }
                else
                {
                    configuration.SetValue(instance, stringValue);
                }
            }
        }

        private void AttemptToConvertAndSetUnconfiguredProperty(
            TTopicParameters instance,
            string variableName,
            string stringValue)
        {
            var propertyInfo = typeof(TTopicParameters)
                .GetProperties()
                .FirstOrDefault(x => x.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo == null)
            {
                throw new InvalidOperationException(
                    $"The class {typeof(TTopicParameters).Name} does not contain a property named {variableName}");
            }

            if (!propertyInfo.CanWrite)
            {
                throw new InvalidOperationException(
                    $"The class {typeof(TTopicParameters).Name}.{propertyInfo.Name} does not have a setter");
            }

            if (!DefaultTypeConverters.PrimitiveTypeConverters.TryGetValue(propertyInfo.PropertyType, out var typeConverter))
            {
                throw new InvalidOperationException(
                    $"There is no type converter registered for {typeof(TTopicParameters).Name}.{propertyInfo.Name}. Use ForProperty() to configure one");
            }

            object objectValue;
            try
            {
                objectValue = typeConverter.Invoke(stringValue);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException(
                    $"Unable to convert value for {typeof(TTopicParameters).Name}.{propertyInfo.Name} from string to {propertyInfo.PropertyType.Name}",
                    exc);
            }

            try
            {
                propertyInfo.SetValue(instance, objectValue);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException(
                    $"Exception encountered while setting {typeof(TTopicParameters).Name}.{propertyInfo.Name}",
                    exc);
            }
        }

        private IDictionary<string, IMqttTopicPatternPropertyConfiguration<TTopicParameters>> Configure(
            IDictionary<string, IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>> propertyConfigurationBuilders)
        {
            var result = new Dictionary<string, IMqttTopicPatternPropertyConfiguration<TTopicParameters>>();
            foreach (var property in propertyConfigurationBuilders)
            {
                if (PropertyConfigurations.TryGetValue(property.Key, out var configurations))
                {
                    foreach (var configuration in configurations)
                    {
                        configuration.Invoke(property.Value);
                    }

                    var config = property.Value.Build();
                    result.Add(property.Key, config);
                }
            }

            return result;
        }
    }
}
