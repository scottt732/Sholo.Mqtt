using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttCorrelationDataAttribute : Attribute
{
    public Type TypeConverterType { get; }

    public FromMqttCorrelationDataAttribute(Type typeConverterType)
    {
        ArgumentNullException.ThrowIfNull(typeConverterType, nameof(typeConverterType));

        if (!typeof(IMqttCorrelationDataTypeConverter).IsAssignableFrom(typeConverterType))
        {
            throw new ArgumentException(
                $"The type {typeConverterType.Name} does not implement {nameof(IMqttCorrelationDataTypeConverter)}",
                nameof(typeConverterType)
            );
        }

        TypeConverterType = typeConverterType;
    }
}
