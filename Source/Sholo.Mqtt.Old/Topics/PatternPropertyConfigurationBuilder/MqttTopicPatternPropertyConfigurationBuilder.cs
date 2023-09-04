using System;
using System.Reflection;
using Sholo.Mqtt.Old.Topics.PatternPropertyConfiguration;
using Sholo.Mqtt.Old.Utilities;

namespace Sholo.Mqtt.Old.Topics.PatternPropertyConfigurationBuilder
{
    public class MqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TParameter> : IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TParameter>
    {
        private string ParameterName { get; set; }
        private MethodInfo ValueSetter { get; }
        private Func<string, object> TypeConverter { get; set; }

        public MqttTopicPatternPropertyConfigurationBuilder(string initialParameterName, Type parameterType, MethodInfo valueSetter)
        {
            ParameterName = initialParameterName;
            ValueSetter = valueSetter;

            if (DefaultTypeConverters.PrimitiveTypeConverters.TryGetValue(parameterType, out var typeConverter))
            {
                TypeConverter = typeConverter;
            }
        }

        public IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TParameter> WithParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        public IMqttTopicPatternPropertyConfigurationBuilder<TTopicParameters, TParameter> WithTypeConverter(Func<string, TParameter> typeConverter)
        {
            TypeConverter = p => typeConverter(p);
            return this;
        }

        public IMqttTopicPatternPropertyConfiguration<TTopicParameters> Build()
        {
            return new MqttTopicPatternPropertyConfiguration<TTopicParameters>(ParameterName, ValueSetter, TypeConverter);
        }
    }
}
