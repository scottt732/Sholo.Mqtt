using System;

namespace Sholo.Mqtt.TypeConverters;

public interface IMqttCorrelationDataTypeConverter
{
    bool TryConvert(byte[] correlationData, Type correlationDataType, out object correlationDataObject);
}
