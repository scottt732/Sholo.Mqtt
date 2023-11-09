#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sholo.Mqtt.TypeConverters;

[PublicAPI]
public static class DefaultTypeConverters
{
    private static Dictionary<Type, Func<string, object?>> StringTypeConverters { get; } =
        new()
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

    private static Dictionary<Type, Func<ArraySegment<byte>, object?>> BinaryTypeConverters { get; } =
        new()
        {
            [typeof(string)] = payload => Encoding.UTF8.GetString(payload),
            [typeof(bool)] = payload => bool.Parse(Encoding.UTF8.GetString(payload)),
            [typeof(char)] = payload => char.Parse(Encoding.UTF8.GetString(payload)),
            [typeof(decimal)] = payload => decimal.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(double)] = payload => double.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(float)] = payload => float.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(int)] = payload => int.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(uint)] = payload => uint.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(long)] = payload => long.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(ulong)] = payload => ulong.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(short)] = payload => short.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(ushort)] = payload => ushort.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture),
            [typeof(Guid)] = payload => Guid.Parse(Encoding.UTF8.GetString(payload)),
            [typeof(bool?)] = payload => payload.Count > 0 ? bool.Parse(Encoding.UTF8.GetString(payload)) : null,
            [typeof(char?)] = payload => payload.Count > 0 ? char.Parse(Encoding.UTF8.GetString(payload)) : null,
            [typeof(decimal?)] = payload => payload.Count > 0 ? decimal.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(double?)] = payload => payload.Count > 0 ? double.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(float?)] = payload => payload.Count > 0 ? float.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(int?)] = payload => payload.Count > 0 ? int.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(uint?)] = payload => payload.Count > 0 ? uint.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(long?)] = payload => payload.Count > 0 ? long.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(ulong?)] = payload => payload.Count > 0 ? ulong.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(short?)] = payload => payload.Count > 0 ? short.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(ushort?)] = payload => payload.Count > 0 ? ushort.Parse(Encoding.UTF8.GetString(payload), CultureInfo.InvariantCulture) : null,
            [typeof(Guid?)] = payload => payload.Count > 0 ? Guid.Parse(Encoding.UTF8.GetString(payload)) : null,
        };

    public static bool TryConvert(string payload, Type targetType, out object? result)
    {
        if (TryGetStringTypeConverter(targetType, out var typeConverter))
        {
            try
            {
                result = typeConverter!.Invoke(payload);
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

    public static bool TryConvert(ArraySegment<byte> payload, Type targetType, out object? result)
    {
        if (TryGetBinaryTypeConverter(targetType, out var typeConverter))
        {
            try
            {
                result = typeConverter!.Invoke(payload);
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

    public static bool TryGetBinaryTypeConverter(Type parameterType, out Func<ArraySegment<byte>, object?>? typeConverter)
    {
        if (typeof(ArraySegment<byte>).IsAssignableTo(parameterType))
        {
            typeConverter = payload => payload;
            return true;
        }

        if (typeof(byte[]).IsAssignableTo(parameterType))
        {
            typeConverter = payload => payload.ToArray();
            return true;
        }

        if (parameterType == typeof(string))
        {
            typeConverter = payload => Encoding.UTF8.GetString(payload);
            return true;
        }

        if (parameterType.IsEnum)
        {
            typeConverter = payload => Enum.Parse(parameterType, Encoding.UTF8.GetString(payload), false);
            return true;
        }

        if (BinaryTypeConverters.TryGetValue(parameterType, out var primitiveTypeConverter))
        {
            typeConverter = primitiveTypeConverter;
            return true;
        }

        typeConverter = null;
        return false;
    }

    public static bool TryGetStringTypeConverter(Type parameterType, out Func<string, object?>? typeConverter)
    {
        if (parameterType == typeof(string))
        {
            typeConverter = str => str;
            return true;
        }

        if (parameterType.IsEnum)
        {
            typeConverter = str => Enum.Parse(parameterType, str, false);
            return true;
        }

        if (StringTypeConverters.TryGetValue(parameterType, out var binaryTypeConverter))
        {
            typeConverter = binaryTypeConverter;
            return true;
        }

        typeConverter = null;
        return false;
    }
}
