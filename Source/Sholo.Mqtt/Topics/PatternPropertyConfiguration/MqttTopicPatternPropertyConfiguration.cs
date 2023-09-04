using System;
using System.Reflection;

namespace Sholo.Mqtt.Topics.PatternPropertyConfiguration
{
    public class MqttTopicPatternPropertyConfiguration : IMqttTopicPatternPropertyConfiguration
    {
        public string ParameterName { get; }
        public bool HaveTypeConverter => TypeConverter != null;

        public object GetParameterValue(string value)
        {
            throw new NotImplementedException();
        }

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
    }
}
