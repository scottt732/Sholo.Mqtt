using System;
using System.Text;

namespace Sholo.Mqtt.TypeConverters;

public abstract class BaseMqttRequestStringTypeConverter : IMqttRequestStringTypeConverter
{
    protected Encoding Encoding { get; }

    protected BaseMqttRequestStringTypeConverter(Encoding encoding = null)
    {
        Encoding = encoding ?? Encoding.UTF8;
    }

    public abstract bool TryConvertString(string sourceData, Type targetType, out object result);

    public bool TryConvertBinary(byte[] sourceData, Type targetType, out object result)
        => TryConvertString(Encoding.GetString(sourceData), targetType, out result);
}
