using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Sholo.Mqtt.Topics.Binder;
using Sholo.Mqtt.Topics.BinderFactoryBuilder;

namespace Sholo.Mqtt.Topics.BinderFactory
{
    [PublicAPI]
    public static class TopicBinderFactory
    {
        public static ITopicBinderFactory<TModel> CreateDefault<TModel>()
            where TModel : class, new()
        {
            return TopicBinderFactoryBuilder.CreateDefault<TModel>().WithAutoProperties().BuildFactory();
        }

        public static ITopicBinderFactory<TModel> CreateDefault<TModel>(Func<TModel> targetFactory)
            where TModel : class
        {
            return TopicBinderFactoryBuilder.CreateDefault(targetFactory).WithAutoProperties().BuildFactory();
        }
    }

    public class TopicBinderFactory<TTargetModel> : ITopicBinderFactory<TTargetModel>
    {
        private Func<TTargetModel> TargetFactory { get; }
        private IDictionary<string, Action<TTargetModel, string>> PropertySetters { get; }
        private ISet<string> UnconfiguredProperties { get; }

        public TopicBinderFactory(
            Func<TTargetModel> targetFactory,
            IDictionary<string, Action<TTargetModel, string>> propertySetters,
            ISet<string> unconfiguredProperties
        )
        {
            TargetFactory = targetFactory;
            PropertySetters = propertySetters;
            UnconfiguredProperties = unconfiguredProperties;
        }

        public ITopicBinder<TTargetModel> CreateBinder(string pattern)
        {
            var topicParts = pattern.Split('/');
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

            ValidatePropertiesHaveSetters(registeredVariables);
            ValidatePropertiesHaveTypeConverters(registeredVariables);

            var regex = new Regex(regBuilder.ToString());

            return new TopicBinder<TTargetModel>(
                TargetFactory,
                PropertySetters,
                regex,
                registeredVariables,
                pattern);
        }

        private void ValidatePropertiesHaveSetters(List<string> registeredVariables)
        {
            StringBuilder sb = null;
            foreach (var unconfiguredVariable in registeredVariables.Where(x => !PropertySetters.ContainsKey(x)))
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

        private void ValidatePropertiesHaveTypeConverters(List<string> registeredVariables)
        {
            StringBuilder sb = null;
            foreach (var unconfiguredVariable in registeredVariables.Where(UnconfiguredProperties.Contains))
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
                throw new ArgumentException("The following properties do not have type converters configured: " + sb);
            }
        }
    }
}
