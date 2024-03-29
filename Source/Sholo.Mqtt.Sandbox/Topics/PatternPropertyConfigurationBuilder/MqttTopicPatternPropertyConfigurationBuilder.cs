using System;
using System.Reflection;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using Sholo.Mqtt.Topics.PatternPropertyConfiguration;

namespace Sholo.Mqtt.Topics.PatternPropertyConfigurationBuilder;

public class MqttTopicPatternPropertyConfigurationBuilder<TParameter> : IMqttTopicPatternPropertyConfigurationBuilder<TParameter>
{
    private string ParameterName { get; set; }
    private MethodInfo ValueSetter { get; }
    private Func<string, object?>? TypeConverter { get; set; }

    public MqttTopicPatternPropertyConfigurationBuilder(string initialParameterName, Type parameterType, MethodInfo valueSetter)
    {
        ParameterName = initialParameterName;
        ValueSetter = valueSetter;

        if (DefaultTypeConverter.TryGetStringTypeConverter(parameterType, out var typeConverter))
        {
            TypeConverter = typeConverter!;
        }
    }

    public IMqttTopicPatternPropertyConfigurationBuilder<TParameter> WithParameterName(string parameterName)
    {
        ParameterName = parameterName;
        return this;
    }

    public IMqttTopicPatternPropertyConfigurationBuilder<TParameter> WithTypeConverter(Func<string, TParameter?> typeConverter)
    {
        TypeConverter = p => typeConverter(p);
        return this;
    }

    public IMqttTopicPatternPropertyConfiguration Build()
    {
        return new MqttTopicPatternPropertyConfiguration(ParameterName, ValueSetter, TypeConverter);
    }
}
