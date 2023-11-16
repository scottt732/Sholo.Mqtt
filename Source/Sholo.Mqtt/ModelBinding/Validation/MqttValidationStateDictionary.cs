// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sholo.Mqtt.ModelBinding.Validation;

/// <summary>
///     Used for tracking validation state to customize validation behavior for a model object.
/// </summary>
[PublicAPI]
public class MqttValidationStateDictionary : IDictionary<object, MqttValidationStateEntry>, IReadOnlyDictionary<object, MqttValidationStateEntry>
{
    private readonly Dictionary<object, MqttValidationStateEntry> _inner;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MqttValidationStateDictionary"/> class.
    /// </summary>
    public MqttValidationStateDictionary()
    {
        _inner = new Dictionary<object, MqttValidationStateEntry>(ReferenceEqualityComparer.Instance);
    }

    /// <summary>
    ///     Gets or sets an item in the dictionary
    /// </summary>
    /// <param name="key">The key of the item to get or set</param>
    public MqttValidationStateEntry? this[object key]
    {
        get
        {
            TryGetValue(key, out var entry);
            return entry;
        }
        set => _inner[key] = value!;
    }

    /// <inheritdoc />
    MqttValidationStateEntry IDictionary<object, MqttValidationStateEntry>.this[object key]
    {
        get => this[key]!;
        set => this[key] = value;
    }

    /// <summary>
    ///     Gets or sets an item in the dictionary
    /// </summary>
    /// <param name="key">The key of the item to get</param>
    MqttValidationStateEntry IReadOnlyDictionary<object, MqttValidationStateEntry>.this[object key] => this[key]!;

    /// <inheritdoc cref="IDictionary.Count" />
    public int Count => _inner.Count;

    /// <inheritdoc />
    public bool IsReadOnly => ((IDictionary<object, MqttValidationStateEntry>)_inner).IsReadOnly;

    /// <inheritdoc />
    public ICollection<object> Keys => ((IDictionary<object, MqttValidationStateEntry>)_inner).Keys;

    /// <inheritdoc />
    public ICollection<MqttValidationStateEntry> Values => ((IDictionary<object, MqttValidationStateEntry>)_inner).Values;

    /// <inheritdoc />
    IEnumerable<object> IReadOnlyDictionary<object, MqttValidationStateEntry>.Keys =>
        ((IReadOnlyDictionary<object, MqttValidationStateEntry>)_inner).Keys;

    /// <inheritdoc />
    IEnumerable<MqttValidationStateEntry> IReadOnlyDictionary<object, MqttValidationStateEntry>.Values =>
        ((IReadOnlyDictionary<object, MqttValidationStateEntry>)_inner).Values;

    /// <inheritdoc />
    public void Add(KeyValuePair<object, MqttValidationStateEntry> item)
    {
        ((IDictionary<object, MqttValidationStateEntry>)_inner).Add(item);
    }

    /// <inheritdoc />
    public void Add(object key, MqttValidationStateEntry value)
    {
        _inner.Add(key, value);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _inner.Clear();
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<object, MqttValidationStateEntry> item)
    {
        return ((IDictionary<object, MqttValidationStateEntry>)_inner).Contains(item);
    }

    /// <inheritdoc cref="IDictionary{K,V}.ContainsKey" />
    public bool ContainsKey(object key)
    {
        return _inner.ContainsKey(key);
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<object, MqttValidationStateEntry>[] array, int arrayIndex)
    {
        ((IDictionary<object, MqttValidationStateEntry>)_inner).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<object, MqttValidationStateEntry>> GetEnumerator()
    {
        return ((IDictionary<object, MqttValidationStateEntry>)_inner).GetEnumerator();
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<object, MqttValidationStateEntry> item)
    {
        return _inner.Remove(item);
    }

    /// <inheritdoc />
    public bool Remove(object key)
    {
        return _inner.Remove(key);
    }

    /// <inheritdoc cref="IDictionary{K,V}.TryGetValue" />
    public bool TryGetValue(object key, [MaybeNullWhen(false)] out MqttValidationStateEntry value)
    {
        return _inner.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IDictionary<object, MqttValidationStateEntry>)_inner).GetEnumerator();
    }
}
