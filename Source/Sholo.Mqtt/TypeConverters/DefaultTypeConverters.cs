using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sholo.Mqtt.TypeConverters;

public static class DefaultTypeConverters
{
    private static IDictionary<Type, Func<string, object>> PrimitiveTypeConverters { get; } =
        new Dictionary<Type, Func<string, object>>
        {
            [typeof(string)] = str => str,
            [typeof(bool)] = str => bool.Parse(str),
            [typeof(char)] = str => char.Parse(str),
            [typeof(decimal)] = str => decimal.Parse(str, CultureInfo.InvariantCulture),
            [typeof(double)] = str => double.Parse(str, CultureInfo.InvariantCulture),
            [typeof(float)] = str => float.Parse(str, CultureInfo.InvariantCulture),
            [typeof(int)] = str => int.Parse(str, CultureInfo.InvariantCulture),
            [typeof(uint)] = str => uint.Parse(str, CultureInfo.InvariantCulture),
            [typeof(long)] = str => long.Parse(str, CultureInfo.InvariantCulture),
            [typeof(ulong)] = str => ulong.Parse(str, CultureInfo.InvariantCulture),
            [typeof(short)] = str => short.Parse(str, CultureInfo.InvariantCulture),
            [typeof(ushort)] = str => ushort.Parse(str, CultureInfo.InvariantCulture),
            [typeof(Guid)] = str => Guid.Parse(str),
            [typeof(bool?)] = str => !string.IsNullOrEmpty(str) ? bool.Parse(str) : null,
            [typeof(char?)] = str => !string.IsNullOrEmpty(str) ? char.Parse(str) : null,
            [typeof(decimal?)] = str => !string.IsNullOrEmpty(str) ? decimal.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(double?)] = str => !string.IsNullOrEmpty(str) ? double.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(float?)] = str => !string.IsNullOrEmpty(str) ? float.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(int?)] = str => !string.IsNullOrEmpty(str) ? int.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(uint?)] = str => !string.IsNullOrEmpty(str) ? uint.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(long?)] = str => !string.IsNullOrEmpty(str) ? long.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(ulong?)] = str => !string.IsNullOrEmpty(str) ? ulong.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(short?)] = str => !string.IsNullOrEmpty(str) ? short.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(ushort?)] = str => !string.IsNullOrEmpty(str) ? ushort.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(Guid?)] = str => !string.IsNullOrEmpty(str) ? Guid.Parse(str) : null,
            [typeof(string)] = str => str
        };

    public static bool TryGetTypeConverter(Type parameterType, out Func<string, object> typeConverter)
    {
        if (parameterType.IsEnum)
        {
            typeConverter = str => Enum.Parse(parameterType, str, false);
            return true;
        }

        if (parameterType == typeof(string))
        {
            typeConverter = str => str;
            return true;
        }

        if (PrimitiveTypeConverters.TryGetValue(parameterType, out typeConverter))
        {
            return true;
        }

        return false;
    }

    public static bool TryConvert(ArraySegment<byte> value, Type targetType, out object result)
    {
        var payload = Encoding.ASCII.GetString(value);
        return TryConvert(payload, targetType, out result);
    }

    public static bool TryConvert(string payload, Type targetType, out object result)
    {
        if (TryGetTypeConverter(targetType, out var typeConverter))
        {
            try
            {
                result = typeConverter.Invoke(payload);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        result = null;
        return false;
    }
}
