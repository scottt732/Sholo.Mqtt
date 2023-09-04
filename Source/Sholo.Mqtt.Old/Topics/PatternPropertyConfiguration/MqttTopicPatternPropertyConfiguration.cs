using System;
using System.Reflection;

namespace Sholo.Mqtt.Old.Topics.PatternPropertyConfiguration
{
    public class MqttTopicPatternPropertyConfiguration<TTopicParameters> : IMqttTopicPatternPropertyConfiguration<TTopicParameters>
    {
        public string ParameterName { get; }
        public bool HaveTypeConverter => TypeConverter != null;
        private MethodInfo ValueSetter { get; }
        private Func<string, object> TypeConverter { get; set; }

        public MqttTopicPatternPropertyConfiguration(
            string parameterName,
            MethodInfo valueSetter,
            Func<string, object> typeConverter)
        {
            ParameterName = parameterName;
            ValueSetter = valueSetter;
            TypeConverter = typeConverter;
        }

        public void SetValue(TTopicParameters target, string value)
            => ValueSetter.Invoke(
                target,
                new[] { TypeConverter.Invoke(value) });
    }
}
