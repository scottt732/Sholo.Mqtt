using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfigurationBuilder;

namespace Sholo.Mqtt.Old.Topics.Filter
{
    public static class MqttTopicFilterExtensions
    {
        public static TTopicParameters Bind<TTopicParameters>(
            this IMqttTopicFilter topicFilter,
            Action<IParametersOptions<TTopicParameters>> parametersConfiguration = null)
            where TTopicParameters : class
        {
            var parametersOptions = new ParametersOptions<TTopicParameters>();
            parametersConfiguration?.Invoke(parametersOptions);

            var regexPattern = topicFilter.ToRegex(out var registeredVariables);
            var instance = parametersOptions.CreateInstance();
            var match = regexPattern.Match(topicFilter.Topic);

            if (!match.Success)
            {
                throw new ArgumentException("The supplied topic did not match the configured pattern");
            }

            if (registeredVariables.Length == 0)
            {
                return instance;
            }

            var propertyConfigurationBuilders = CreatePropertyConfigurationBuilders<TTopicParameters>();

            // match.Success implies that every registered variable has a corresponding Group in the match with a value.
            // The regex does not have optional parameters. So match.Groups[x].Value should never throw.
            var variableStringValues = registeredVariables
                .ToDictionary(
                    x => x,
                    x => match.Groups[x].Value);

            parametersOptions.BindParameters(instance, propertyConfigurationBuilders, variableStringValues);
            return instance;
        }

        private static Regex ToRegex(this IMqttTopicFilter topicFilter, out string[] registeredVariables)
        {
            if (topicFilter == null)
            {
                throw new ArgumentNullException(nameof(topicFilter), $"The {nameof(topicFilter)} cannot be null");
            }

            if (topicFilter.Topic == null)
            {
                throw new ArgumentNullException(nameof(topicFilter.Topic), $"The {nameof(topicFilter)}.{nameof(topicFilter.Topic)} cannot be null");
            }

            if (topicFilter.Topic.Length == 0)
            {
                throw new ArgumentException(
                    $"The {nameof(topicFilter)}.${nameof(topicFilter.Topic)} must be non-empty",
                    nameof(topicFilter));
            }

            // TODO: Rename Topic to TopicPattern
            var topicParts = topicFilter.Topic.Split('/');
            var regBuilder = new StringBuilder();

            var haveMultiLevelWildcard = false;

            var registeredVariablesList = new List<string>();
            for (var i = 0; i < topicParts.Length; i++)
            {
                var topicPart = topicParts[i];
                if (topicPart.StartsWith("+", StringComparison.Ordinal))
                {
                    var variableName = topicPart.Substring(1);
                    registeredVariablesList.Add(variableName);

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
                    registeredVariablesList.Add(variableName);

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

            registeredVariables = registeredVariablesList.ToArray();
            return new Regex(regBuilder.ToString());
        }

        private static IDictionary<string, IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>> CreatePropertyConfigurationBuilders<TTopicParameters>()
            where TTopicParameters : class
        {
            var propertyConfigurationBuildersByClassPropertyName = typeof(TTopicParameters)
                .GetProperties()
                .Select(x => (PropertyName: x.Name, x.PropertyType, ValueSetter: x.GetSetMethod()))
                .Where(x => x.ValueSetter != null && x.ValueSetter.IsPublic)
                .ToDictionary(
                    x => x.PropertyName,
                    x => CreateBuilder<TTopicParameters>(x.PropertyName, x.PropertyType, x.ValueSetter));

            return propertyConfigurationBuildersByClassPropertyName;
        }

        private static IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters> CreateBuilder<TTopicParameters>(
            string propertyName,
            Type parameterType,
            MethodInfo valueSetter)
        {
            var resultType = typeof(MqttTopicPatternPropertyConfigurationBuilder<,>).MakeGenericType(typeof(TTopicParameters), parameterType);
            var constructor = resultType.GetConstructor(new[] { typeof(string), typeof(Type), typeof(MethodInfo) });

            var instanceObject = constructor?.Invoke(new object[] { propertyName, parameterType, valueSetter })
                ?? throw new InvalidOperationException($"Unable to construct {nameof(TTopicParameters)}.{propertyName} or type {parameterType.Name}");

            var instance = (IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>)instanceObject;

            return instance;
        }
    }
}
