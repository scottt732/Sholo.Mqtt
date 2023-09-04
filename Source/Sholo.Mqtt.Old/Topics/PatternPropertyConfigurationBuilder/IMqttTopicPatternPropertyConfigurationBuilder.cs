using System;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfiguration;

namespace Sholo.Mqtt.Old.Topics.PatternPropertyConfigurationBuilder
{
    public interface IMqttTopicPatternPropertyConfigurationBuilder<in TTopicParameters>
    {
        IMqttTopicPatternPropertyConfiguration<TTopicParameters> Build();
    }

    public interface IMqttTopicPatternPropertyConfigurationBuilder<in TTopicParameters, in TParameter> : IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters>
    {
        IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TParameter> WithParameterName(string parameterName);
        IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TParameter> WithTypeConverter(Func<string, TParameter> typeConverter);
    }
}
