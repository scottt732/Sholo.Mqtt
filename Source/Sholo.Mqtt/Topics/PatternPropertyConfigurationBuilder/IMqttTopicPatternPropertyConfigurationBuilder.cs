using System;
using Sholo.Mqtt.Topics.PatternPropertyConfiguration;

namespace Sholo.Mqtt.Topics.PatternPropertyConfigurationBuilder;

public interface IMqttTopicPatternPropertyConfigurationBuilder
{
    IMqttTopicPatternPropertyConfiguration Build();
}

public interface IMqttTopicPatternPropertyConfigurationBuilder<in TParameter> : IMqttTopicPatternPropertyConfigurationBuilder
{
    IMqttTopicPatternPropertyConfigurationBuilder<TParameter> WithParameterName(string parameterName);
    IMqttTopicPatternPropertyConfigurationBuilder<TParameter> WithTypeConverter(Func<string, TParameter> typeConverter);
}
