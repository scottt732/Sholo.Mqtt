using System;

namespace Sholo.Mqtt.TypeConverters.Parameter
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromMqttTopicAttribute : Attribute
    {
        public Type TypeConverterType { get; }

        public FromMqttTopicAttribute(Type typeConverterType)
        {
            if (typeConverterType == null)
            {
                throw new ArgumentNullException(nameof(typeConverterType));
            }

            if (!typeof(IMqttParameterTypeConverter).IsAssignableFrom(typeConverterType))
            {
                throw new ArgumentException(
                    $"The type {typeConverterType.Name} does not implement {nameof(IMqttParameterTypeConverter)}",
                    nameof(typeConverterType)
                );
            }

            TypeConverterType = typeConverterType;
        }
    }
}
