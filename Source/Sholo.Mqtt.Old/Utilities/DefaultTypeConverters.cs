using System;
using System.Collections.Generic;

namespace Sholo.Mqtt.Old.Utilities
{
    internal static class DefaultTypeConverters
    {
        public static IDictionary<Type, Func<string, object>> PrimitiveTypeConverters { get; } =
            new Dictionary<Type, Func<string, object>>
            {
                [typeof(bool)] = str => bool.Parse(str),
                [typeof(char)] = str => char.Parse(str),
                [typeof(decimal)] = str => decimal.Parse(str),
                [typeof(double)] = str => double.Parse(str),
                [typeof(float)] = str => float.Parse(str),
                [typeof(int)] = str => int.Parse(str),
                [typeof(uint)] = str => uint.Parse(str),
                [typeof(long)] = str => long.Parse(str),
                [typeof(ulong)] = str => ulong.Parse(str),
                [typeof(short)] = str => short.Parse(str),
                [typeof(ushort)] = str => ushort.Parse(str),
                [typeof(Guid)] = str => Guid.Parse(str),
                [typeof(bool?)] = str => !string.IsNullOrEmpty(str) ? bool.Parse(str) : null,
                [typeof(char?)] = str => !string.IsNullOrEmpty(str) ? char.Parse(str) : null,
                [typeof(decimal?)] = str => !string.IsNullOrEmpty(str) ? decimal.Parse(str) : null,
                [typeof(double?)] = str => !string.IsNullOrEmpty(str) ? double.Parse(str) : null,
                [typeof(float?)] = str => !string.IsNullOrEmpty(str) ? float.Parse(str) : null,
                [typeof(int?)] = str => !string.IsNullOrEmpty(str) ? int.Parse(str) : null,
                [typeof(uint?)] = str => !string.IsNullOrEmpty(str) ? uint.Parse(str) : null,
                [typeof(long?)] = str => !string.IsNullOrEmpty(str) ? long.Parse(str) : null,
                [typeof(ulong?)] = str => !string.IsNullOrEmpty(str) ? ulong.Parse(str) : null,
                [typeof(short?)] = str => !string.IsNullOrEmpty(str) ? short.Parse(str) : null,
                [typeof(ushort?)] = str => !string.IsNullOrEmpty(str) ? ushort.Parse(str) : null,
                [typeof(Guid?)] = str => !string.IsNullOrEmpty(str) ? Guid.Parse(str) : null,
                [typeof(string)] = str => str
            };
    }
}
