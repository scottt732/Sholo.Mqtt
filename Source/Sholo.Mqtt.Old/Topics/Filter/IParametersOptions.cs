using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfigurationBuilder;

namespace Sholo.Mqtt.Old.Topics.Filter
{
    [PublicAPI]
    public interface IParametersOptions<TTopicParameters>
        where TTopicParameters : class
    {
        IParametersOptions<TTopicParameters> WithFactory(Func<TTopicParameters> topicParametersFactory);

        IParametersOptions<TTopicParameters> ForProperty<TProperty>(
            Expression<Func<TTopicParameters, TProperty>> propertySelector,
            Action<IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>> configuration);
    }
}
