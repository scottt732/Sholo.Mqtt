using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

[PublicAPI]
public interface IMqttCorrelationDataTypeConverter : IMqttTypeConverter
{
    bool TryConvertCorrelationData(byte[]? correlationData, Type targetType, out object? result);
}
