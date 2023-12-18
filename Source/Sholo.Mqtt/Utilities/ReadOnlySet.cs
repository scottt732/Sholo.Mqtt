using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sholo.Mqtt.Utilities;

[PublicAPI]
public static class ReadOnlySet
{
    private static readonly ConcurrentDictionary<Type, object> Instances = new();

    public static IReadOnlySet<T> Empty<T>() => (IReadOnlySet<T>)Instances.GetOrAdd(typeof(T), _ => new ReadOnlySet<T>(new HashSet<T>()));
}

[PublicAPI]
public class ReadOnlySet<T> : IReadOnlySet<T>
{
    private ISet<T> Set { get; }

    public ReadOnlySet(ISet<T> set)
    {
        Set = set ?? throw new ArgumentNullException(nameof(set));
    }

    public int Count => Set.Count;
    public bool Contains(T item) => Set.Contains(item);
    public bool IsProperSubsetOf(IEnumerable<T> other) => Set.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<T> other) => Set.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<T> other) => Set.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<T> other) => Set.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<T> other) => Set.Overlaps(other);
    public bool SetEquals(IEnumerable<T> other) => Set.SetEquals(other);
    public IEnumerator<T> GetEnumerator() => Set.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
