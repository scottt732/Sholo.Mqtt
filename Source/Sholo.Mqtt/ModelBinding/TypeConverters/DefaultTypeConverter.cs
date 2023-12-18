using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

[PublicAPI]
public class DefaultTypeConverter : IMqttCorrelationDataTypeConverter, IMqttPayloadTypeConverter, IMqttTopicArgumentTypeConverter, IMqttUserPropertiesTypeConverter
{
    public static DefaultTypeConverter Instance => InstanceFactory.Value;
    private static Lazy<DefaultTypeConverter> InstanceFactory { get; } = new(() => new DefaultTypeConverter());

    public bool TryConvertCorrelationData(byte[]? correlationData, Type targetType, out object? result)
        => TryConvert(new ArraySegment<byte>(correlationData ?? Array.Empty<byte>()), targetType, out result);

    public bool TryConvertPayload(ArraySegment<byte> payload, Type targetType, out object? result)
        => TryConvert(payload, targetType, out result);

    public bool TryConvertTopicArgument(string argument, Type targetType, out object? result)
        => TryConvert(argument, targetType, out result);

    public bool TryConvertUserPropertyValues(StringValues? values, Type targetType, out IList<object?>? result)
    {
        if (!values.HasValue)
        {
            result = null;
            return false;
        }

        var list = new List<object?>();
        foreach (var value in values)
        {
            if (!TryConvert(values, targetType, out var itemResult))
            {
                result = null;
                return false;
            }
            else
            {
                list.Add(itemResult);
            }
        }

        result = list;
        return true;
    }

    private static Dictionary<Type, Func<string?, object?>> StringTypeConverters { get; } =
        new()
        {
            [typeof(byte)] = str => byte.Parse(str ?? throw new FormatException("The string supplied was null")),
            [typeof(bool)] = str => bool.Parse(str ?? throw new FormatException("The string supplied was null")),
            [typeof(char)] = str => char.Parse(str ?? throw new FormatException("The string supplied was null")),
            [typeof(decimal)] = str => decimal.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(double)] = str => double.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(float)] = str => float.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(int)] = str => int.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(uint)] = str => uint.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(long)] = str => long.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(ulong)] = str => ulong.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(short)] = str => short.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(ushort)] = str => ushort.Parse(str ?? throw new FormatException("The string supplied was null"), CultureInfo.InvariantCulture),
            [typeof(Guid)] = str => Guid.Parse(str ?? throw new FormatException("The string supplied was null")),
            [typeof(byte?)] = str => str != null ? byte.Parse(str) : null,
            [typeof(bool?)] = str => str != null ? bool.Parse(str) : null,
            [typeof(char?)] = str => str != null ? char.Parse(str) : null,
            [typeof(decimal?)] = str => str != null ? decimal.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(double?)] = str => str != null ? double.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(float?)] = str => str != null ? float.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(int?)] = str => str != null ? int.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(uint?)] = str => str != null ? uint.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(long?)] = str => str != null ? long.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(ulong?)] = str => str != null ? ulong.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(short?)] = str => str != null ? short.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(ushort?)] = str => str != null ? ushort.Parse(str, CultureInfo.InvariantCulture) : null,
            [typeof(Guid?)] = str => str != null ? Guid.Parse(str) : null,
        };

    private static Dictionary<Type, Func<ArraySegment<byte>, object?>> BinaryTypeConverters { get; } =
        new()
        {
            [typeof(byte)] = payload => byte.Parse(Encoding.UTF8.GetString(payload)),
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
            [typeof(byte?)] = payload => payload.Count > 0 ? byte.Parse(Encoding.UTF8.GetString(payload)) : null,
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
            typeConverter = payload => payload != null! ? Encoding.UTF8.GetString(payload) : null!;
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(parameterType);
        if (underlyingType is { IsEnum: true })
        {
            typeConverter = payload => payload != null! ? Enum.Parse(underlyingType, Encoding.UTF8.GetString(payload)) : null!;
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

    public static bool TryGetStringTypeConverter(Type parameterType, out Func<string?, object?>? typeConverter)
    {
        if (parameterType == typeof(string))
        {
            typeConverter = str => str;
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(parameterType);
        if (underlyingType is { IsEnum: true })
        {
            typeConverter = str => str != null ? Enum.Parse(underlyingType, str) : null;
            return true;
        }

        if (parameterType.IsEnum)
        {
            typeConverter = str => Enum.Parse(parameterType, str ?? throw new FormatException("The string supplied was null"), false);
            return true;
        }

        if (StringTypeConverters.TryGetValue(parameterType, out var stringTypeConverter))
        {
            typeConverter = stringTypeConverter;
            return true;
        }

        typeConverter = null;
        return false;
    }

    public bool TryConvert(string? input, Type targetType, out object? result)
    {
        if (targetType.IsInstanceOfType(input))
        {
            result = input;
            return true;
        }

        if (TryGetStringTypeConverter(targetType, out var typeConverter))
        {
            try
            {
                result = typeConverter!.Invoke(input);
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

    public bool TryConvert(ArraySegment<byte> input, Type targetType, out object? result)
    {
        if (targetType.IsInstanceOfType(input))
        {
            result = input;
            return true;
        }

        if (TryGetBinaryTypeConverter(targetType, out var typeConverter))
        {
            try
            {
                result = typeConverter!.Invoke(input);
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

    public bool TryConvert(byte[]? input, Type targetType, out object? result)
    {
        if (input == null)
        {
            result = null;
            return false;
        }

        return TryConvert(new ArraySegment<byte>(input), targetType, out result);
    }
}
