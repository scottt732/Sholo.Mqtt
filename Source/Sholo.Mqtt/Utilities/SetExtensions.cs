using System.Collections.Generic;

namespace Sholo.Mqtt.Utilities;

[PublicAPI]
public static class SetExtensions
{
    public static IReadOnlySet<T> AsReadOnly<T>(this ISet<T> set) => new ReadOnlySet<T>(set);
}
