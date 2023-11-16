using System;
using System.Diagnostics;

namespace Sholo.Mqtt.ModelBinding;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/// <summary>
///     A metadata object representing a source of data for model binding.
/// </summary>
/// <remarks>
///     This is largely copy-and-paste from https://github.com/dotnet/aspnetcore
/// </remarks>
[PublicAPI]
[DebuggerDisplay("Source = {Id}")]
public class MqttBindingSource : IEquatable<MqttBindingSource?>
{
    /// <summary>
    ///     A <see cref="MqttBindingSource"/> for the request payload.
    /// </summary>
    /// <remarks>
    ///     See <see cref="IMqttRequestContext.Payload" />
    /// </remarks>
    public static readonly MqttBindingSource Payload = new(nameof(Payload), isFromRequest: true);

    /// <summary>
    ///     A <see cref="MqttBindingSource"/> for the request correlation data.
    /// </summary>
    /// <remarks>
    ///     See <see cref="IMqttRequestContext.CorrelationData" />
    /// </remarks>
    public static readonly MqttBindingSource CorrelationData = new(nameof(CorrelationData), isFromRequest: true);

    /// <summary>
    ///     A <see cref="MqttBindingSource"/> for the MQTT user properties
    /// </summary>
    /// <remarks>
    ///     See <see cref="IMqttRequestContext.UserProperties" />
    /// </remarks>
    public static readonly MqttBindingSource UserProperties = new(nameof(UserProperties), isFromRequest: true);

    /// <summary>
    ///     A <see cref="MqttBindingSource"/> for model binding. Includes arguments extracted from
    ///     the MQTT topic
    /// </summary>
    /// <remarks>
    ///     See <see cref="IMqttRequestContext.Topic" />
    /// </remarks>
    public static readonly MqttBindingSource Topic = new(nameof(Topic), isFromRequest: true);

    /// <summary>
    ///     A <see cref="MqttBindingSource"/> for the <see cref="IMqttRequestContext" />
    /// </summary>
    public static readonly MqttBindingSource Context = new(nameof(Context), isFromRequest: true);

    /// <summary>
    ///     Gets the unique identifier for the source. Sources are compared based on their Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     Gets a value indicating whether or not the binding source uses input from the current HTTP request.
    /// </summary>
    public bool IsFromRequest { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MqttBindingSource"/> class.
    /// </summary>
    /// <param name="id">The id, a unique identifier.</param>
    /// <param name="isFromRequest">A value indicating whether or not the data comes from the HTTP request.</param>
    public MqttBindingSource(string id, bool isFromRequest)
    {
        ArgumentNullException.ThrowIfNull(id);

        Id = id;
        IsFromRequest = isFromRequest;
    }

    /// <inheritdoc />
    public bool Equals(MqttBindingSource? other)
    {
        return string.Equals(other?.Id, Id, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as MqttBindingSource);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.GetHashCode(StringComparison.InvariantCulture);
    }

    public static bool operator ==(MqttBindingSource? s1, MqttBindingSource? s2)
    {
        if (s1 is null)
        {
            return s2 is null;
        }

        return s1.Equals(s2);
    }

    public static bool operator !=(MqttBindingSource? s1, MqttBindingSource? s2)
    {
        return !(s1 == s2);
    }
}
