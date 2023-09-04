using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MQTTnet.Protocol;
using Sholo.Mqtt.Old.Topics.FilterSanitizer;
using Sholo.Mqtt.Old.Topics.PatternFilter;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfiguration;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfigurationBuilder;

namespace Sholo.Mqtt.Old.Topics.PatternFilterBuilder
{
    // Removing in favor of adding a `Bind<TTopicParameters>()` extension method to IMqttTopicFilter,
    // TTopicParameters' property configuration methods to DI layer (IMqttServiceCollection).
    // This keeps IMqttApplicationBuilder simpler

    internal class MqttTopicPatternFilterBuilder<TTopicParameters> : IMqttTopicPatternFilterBuilder<TTopicParameters>
        where TTopicParameters : class
    {
        private string Topic { get; set; }
        private string TopicPattern { get; set; }
        private MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }
        private bool? NoLocal { get; set; }
        private bool? RetainAsPublished { get; set; }
        private MqttRetainHandling? RetainHandling { get; set; }
        private Func<TTopicParameters> TopicParametersFactory { get; set; }

        private IDictionary<string, IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>> PropertyConfigurationBuildersByClassPropertyName { get; }

        public MqttTopicPatternFilterBuilder()
        {
            PropertyConfigurationBuildersByClassPropertyName = typeof(TTopicParameters)
                .GetProperties()
                .Select(x => (PropertyName: x.Name, x.PropertyType, ValueSetter: x.GetSetMethod()))
                .Where(x => x.ValueSetter != null)
                .Where(x => x.ValueSetter.IsPublic)
                .ToDictionary(
                    x => x.PropertyName,
                    x => CreateBuilder(x.PropertyName, x.PropertyType, x.ValueSetter));
        }

        public IMqttTopicPatternFilterBuilder<TTopicParameters> WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel)
        {
            QualityOfServiceLevel = qualityOfServiceLevel;
            return this;
        }

        public IMqttTopicPatternFilterBuilder<TTopicParameters> WithTopicPattern(string topicPattern)
        {
            Topic = TopicFilterSanitizer.SanitizeTopic(topicPattern);
            TopicPattern = topicPattern;
            return this;
        }

        public IMqttTopicPatternFilterBuilder<TTopicParameters> WithNoLocal(bool noLocal)
        {
            NoLocal = noLocal;
            return this;
        }

        public IMqttTopicPatternFilterBuilder<TTopicParameters> WithRetainAsPublished(bool retainAsPublished)
        {
            RetainAsPublished = retainAsPublished;
            return this;
        }

        public IMqttTopicPatternFilterBuilder<TTopicParameters> WithRetainHandling(MqttRetainHandling retainHandling)
        {
            RetainHandling = retainHandling;
            return this;
        }

        public IMqttTopicPatternFilterBuilder<TTopicParameters> WithTopicParametersFactory(Func<TTopicParameters> topicParametersFactory)
        {
            TopicParametersFactory = topicParametersFactory;
            return this;
        }

        public IMqttTopicPatternFilterBuilder<TTopicParameters> WithParameter<TProperty>(
            Expression<Func<TTopicParameters, TProperty>> propertySelector,
            Action<IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TProperty>> configuration
        )
        {
            var memberExpression = (MemberExpression)propertySelector.Body;
            var property = (PropertyInfo)memberExpression.Member;

            if (!PropertyConfigurationBuildersByClassPropertyName.TryGetValue(property.Name, out var propertyConfigurationBuilder))
            {
                throw new InvalidOperationException($"The topic class {typeof(TTopicParameters).Name} does not have a public setter for the property {property.Name}");
            }

            var typedPropertyConfigurationBuilder = (IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TProperty>)propertyConfigurationBuilder;
            configuration?.Invoke(typedPropertyConfigurationBuilder);

            return this;
        }

        public IMqttTopicPatternFilter<TTopicParameters> Build()
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
            var regBuilder = new StringBuilder();

            var haveMultiLevelWildcard = false;

            var registeredVariables = new List<string>();
            for (var i = 0; i < topicParts.Length; i++)
            {
                var topicPart = topicParts[i];
                if (topicPart.StartsWith("+", StringComparison.Ordinal))
                {
                    var variableName = topicPart.Substring(1);
                    registeredVariables.Add(variableName);

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
                    registeredVariables.Add(variableName);

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

            var propertyConfigurationsByTopicParameterName = GetPropertyConfigurationsByTopicParameterName();

            ValidatePropertiesHaveSetters(registeredVariables, propertyConfigurationsByTopicParameterName);
            ValidatePropertiesHaveTypeConverters(registeredVariables, propertyConfigurationsByTopicParameterName);

            var regex = new Regex(regBuilder.ToString());

            var result = new MqttTopicPatternFilter<TTopicParameters>(
                Topic,
                TopicPattern,
                TopicParametersFactory ?? Activator.CreateInstance<TTopicParameters>,
                regex,
                registeredVariables,
                propertyConfigurationsByTopicParameterName,
                QualityOfServiceLevel,
                NoLocal,
                RetainAsPublished,
                RetainHandling);

            return result;
        }

        private IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters> CreateBuilder(
            string propertyName,
            Type parameterType,
            MethodInfo valueSetter)
        {
            var resultType = typeof(MqttTopicPatternPropertyConfigurationBuilder<,>).MakeGenericType(typeof(TTopicParameters), parameterType);
            var constructor = resultType.GetConstructor(new Type[] { typeof(string), typeof(Type), typeof(MethodInfo) });
            var instanceObject = constructor.Invoke(new object[] { propertyName, parameterType, valueSetter });
            var instance = (IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>)instanceObject;

            return instance;
        }

        private IDictionary<string, IMqttTopicPatternPropertyConfiguration<TTopicParameters>> GetPropertyConfigurationsByTopicParameterName()
        {
            return PropertyConfigurationBuildersByClassPropertyName
                .Select(x => x.Value.Build())
                .ToDictionary(
                    x => x.ParameterName,
                    x => x);
        }

        private void ValidatePropertiesHaveSetters(
            IList<string> registeredVariables,
            IDictionary<string, IMqttTopicPatternPropertyConfiguration<TTopicParameters>> propertyConfigurationsByTopicParameterName)
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
            IDictionary<string, IMqttTopicPatternPropertyConfiguration<TTopicParameters>> propertyConfigurationsByTopicParameterName)
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
