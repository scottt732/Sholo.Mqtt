namespace Sholo.Mqtt.Utilities;

/*
public static class ReadOnlyDictionary
{
    private static readonly ConcurrentDictionary<(Type, Type), object> Instances = new();

    public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull => (IReadOnlyDictionary<TKey, TValue>)Instances.GetOrAdd((typeof(TKey), typeof(TValue)), _ => new ReadOnlyDictionary<TKey, TValue>());

    public static IReadOnlyDictionary<TKey, TValue> => new ReadOnlyDictionary<TKey, TValue>(items.ToDictionary(x => x.Key, x => x.Value));>
}
*/
