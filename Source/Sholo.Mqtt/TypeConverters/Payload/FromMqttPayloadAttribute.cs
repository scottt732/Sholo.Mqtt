using System;

namespace Sholo.Mqtt.TypeConverters.Payload;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttPayloadAttribute : Attribute
{
    public Type TypeConverterType { get; }

    public FromMqttPayloadAttribute(Type typeConverterType)
    {
        ArgumentNullException.ThrowIfNull(typeConverterType, nameof(typeConverterType));

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
