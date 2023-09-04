using System;
using System.Collections;
using System.Collections.Generic;

namespace Sholo.Mqtt.Internal
{
    internal class CopyOnWriteDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly IDictionary<TKey, TValue> _sourceDictionary;
        private readonly IEqualityComparer<TKey> _comparer;
        private IDictionary<TKey, TValue> _innerDictionary;

        public virtual ICollection<TKey> Keys => ReadDictionary.Keys;
        public virtual ICollection<TValue> Values => ReadDictionary.Values;
        public virtual int Count => ReadDictionary.Count;
        public virtual bool IsReadOnly => false;

        public virtual TValue this[TKey key]
        {
            get => ReadDictionary[key];
            set => WriteDictionary[key] = value;
        }

        private IDictionary<TKey, TValue> ReadDictionary => _innerDictionary ?? _sourceDictionary;
        private IDictionary<TKey, TValue> WriteDictionary => _innerDictionary ??= new Dictionary<TKey, TValue>(_sourceDictionary, _comparer);

        public CopyOnWriteDictionary(
            IDictionary<TKey, TValue> sourceDictionary,
            IEqualityComparer<TKey> comparer)
        {
            _sourceDictionary = sourceDictionary ?? throw new ArgumentNullException(nameof(sourceDictionary));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public virtual bool ContainsKey(TKey key) => ReadDictionary.ContainsKey(key);
        public virtual void Add(TKey key, TValue value) => WriteDictionary.Add(key, value);
        public virtual bool Remove(TKey key) => WriteDictionary.Remove(key);
        public virtual bool TryGetValue(TKey key, out TValue value) => ReadDictionary.TryGetValue(key, out value);
        public virtual void Add(KeyValuePair<TKey, TValue> item) => WriteDictionary.Add(item);
        public virtual void Clear() => WriteDictionary.Clear();
        public virtual bool Contains(KeyValuePair<TKey, TValue> item) => ReadDictionary.Contains(item);
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ReadDictionary.CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => WriteDictionary.Remove(item);
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ReadDictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
