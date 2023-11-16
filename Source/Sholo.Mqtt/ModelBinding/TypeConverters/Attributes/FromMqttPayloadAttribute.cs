using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttPayloadAttribute : Attribute
{
    public Type TypeConverterType { get; }

    public FromMqttPayloadAttribute(Type typeConverterType)
    {
        ArgumentNullException.ThrowIfNull(typeConverterType, nameof(typeConverterType));

        if (!typeof(IMqttPayloadTypeConverter).IsAssignableFrom(typeConverterType))
        {
            throw new ArgumentException(
                $"The type {typeConverterType.Name} does not implement {nameof(IMqttPayloadTypeConverter)}",
                nameof(typeConverterType)
            );
        }

        TypeConverterType = typeConverterType;
    }
}
