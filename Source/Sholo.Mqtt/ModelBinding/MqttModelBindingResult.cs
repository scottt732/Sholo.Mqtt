// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Sholo.Mqtt.ModelBinding;

/// <summary>
///     Contains the result of model binding.
/// </summary>
[PublicAPI]
public readonly struct MqttModelBindingResult : IEquatable<MqttModelBindingResult>
{
    /// <summary>
    ///     Creates a <see cref="MqttModelBindingResult"/> representing a failed model binding operation.
    /// </summary>
    /// <returns>A <see cref="MqttModelBindingResult"/> representing a failed model binding operation.</returns>
    public static MqttModelBindingResult Failed()
    {
        return new MqttModelBindingResult(model: null, isModelSet: false);
    }

    /// <summary>
    ///     Creates a <see cref="MqttModelBindingResult"/> representing a successful model binding operation.
    /// </summary>
    /// <param name="model">The model value. May be <c>null.</c></param>
    /// <returns>A <see cref="MqttModelBindingResult"/> representing a successful model bind.</returns>
    public static MqttModelBindingResult Success(object? model)
    {
        return new MqttModelBindingResult(model, isModelSet: true);
    }

    private MqttModelBindingResult(object? model, bool isModelSet)
    {
        Model = model;
        IsModelSet = isModelSet;
    }

    /// <summary>
    ///     Gets the model associated with this context.
    /// </summary>
    public object? Model { get; }

    /// <summary>
    ///     <para>
    ///         Gets a value indicating whether or not the <see cref="Model" /> value has been set.
    ///     </para>
    ///     <para>
    ///         This property can be used to distinguish between a model binder which does not find a value and
    ///         the case where a model binder sets the <c>null</c> value.
    ///     </para>
    /// </summary>
    public bool IsModelSet { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        var other = obj as MqttModelBindingResult?;
        if (other == null)
        {
            return false;
        }
        else
        {
            return Equals(other.Value);
        }
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(IsModelSet, Model);
    }

    /// <inheritdoc />
    public bool Equals(MqttModelBindingResult other)
    {
        // ReSharper disable once RedundantNameQualifier
        return
            IsModelSet == other.IsModelSet &&
            object.Equals(Model, other.Model);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsModelSet)
        {
            return $"Success '{Model}'";
        }
        else
        {
            return "Failed";
        }
    }

    /// <summary>
    ///     Compares <see cref="MqttModelBindingResult"/> objects for equality.
    /// </summary>
    /// <param name="x">A <see cref="MqttModelBindingResult"/>.</param>
    /// <param name="y">A <see cref="MqttModelBindingResult"/> to compare with.</param>
    /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
    public static bool operator ==(MqttModelBindingResult x, MqttModelBindingResult y)
    {
        return x.Equals(y);
    }

    /// <summary>
    /// Compares <see cref="MqttModelBindingResult"/> objects for inequality.
    /// </summary>
    /// <param name="x">A <see cref="MqttModelBindingResult"/>.</param>
    /// <param name="y">A <see cref="MqttModelBindingResult"/> to compare with.</param>
    /// <returns><c>true</c> if the objects are not equal, otherwise <c>false</c>.</returns>
    public static bool operator !=(MqttModelBindingResult x, MqttModelBindingResult y)
    {
        return !x.Equals(y);
    }
}
