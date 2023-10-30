using System;

namespace Sholo.Mqtt.TypeConverters.Payload;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttPayloadAttribute : Attribute
{
    public Type TypeConverterType { get; }

    public FromMqttPayloadAttribute(Type typeConverterType)
    {
        if (typeConverterType == null)
        {
            throw new ArgumentNullException(nameof(typeConverterType));
        }

        if (!typeof(IMqttRequestPayloadTypeConverter).IsAssignableFrom(typeConverterType))
        {
            throw new ArgumentException(
                $"The type {typeConverterType.Name} does not implement {nameof(IMqttRequestPayloadTypeConverter)}",
                nameof(typeConverterType)
            );
        }

        TypeConverterType = typeConverterType;
    }
}
