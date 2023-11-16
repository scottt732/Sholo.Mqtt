// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

/// <summary>
///     Result of an <see cref="IMqttValueProvider.GetValue"/> operation.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="MqttValueProviderResult"/> can represent a single submitted value or multiple submitted values.
///     </para>
///     <para>
///         Use <see cref="FirstValue"/> to consume only a single value, regardless of whether a single value or
///         multiple values were submitted.
///     </para>
///     <para>
///         Treat <see cref="MqttValueProviderResult"/> as an <see cref="IEnumerable{String}"/> to consume all values,
///         regardless of whether a single value or multiple values were submitted.
///     </para>
/// </remarks>
/// <remarks>
///     This is largely copy-and-paste from https://github.com/dotnet/aspnetcore
/// </remarks>
[PublicAPI]
public readonly struct MqttValueProviderResult : IEquatable<MqttValueProviderResult>, IEnumerable<string>
{
    /// <summary>
    ///     A <see cref="MqttValueProviderResult"/> that represents a lack of data.
    /// </summary>
    public static readonly MqttValueProviderResult None = new(Array.Empty<string>());

    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MqttValueProviderResult"/> struct using <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="values">The submitted values.</param>
    public MqttValueProviderResult(StringValues values)
        : this(values, InvariantCulture)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MqttValueProviderResult"/> struct.
    /// </summary>
    /// <param name="values">The submitted values.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> associated with this value.</param>
    public MqttValueProviderResult(StringValues values, CultureInfo? culture)
    {
        Values = values;
        Culture = culture ?? InvariantCulture;
    }

    /// <summary>
    ///     Gets the <see cref="CultureInfo"/> associated with the values.
    /// </summary>
    public CultureInfo Culture { get; }

    /// <summary>
    ///     Gets the values.
    /// </summary>
    public StringValues Values { get; }

    /// <summary>
    ///     Gets the first value based on the order values were provided in the request. Use <see cref="FirstValue"/>
    ///     to get a single value for processing regardless of whether a single or multiple values were provided
    ///     in the request.
    /// </summary>
    public string? FirstValue
    {
        get
        {
            if (Values.Count == 0)
            {
                return null;
            }

            return Values[0];
        }
    }

    /// <summary>
    ///     Gets the number of submitted values.
    /// </summary>
    public int Length => Values.Count;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        var other = obj as MqttValueProviderResult?;
        return other.HasValue && Equals(other.Value);
    }

    /// <inheritdoc />
    public bool Equals(MqttValueProviderResult other)
    {
        if (Length != other.Length)
        {
            return false;
        }
        else
        {
            var x = Values;
            var y = other.Values;
            for (var i = 0; i < x.Count; i++)
            {
                if (!string.Equals(x[i], y[i], StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ToString().GetHashCode(StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Values.ToString();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Gets an <see cref="IEnumerator{String}"/> for this <see cref="MqttValueProviderResult"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{String}"/>.</returns>
    public IEnumerator<string> GetEnumerator()
    {
        return ((IEnumerable<string>)Values).GetEnumerator();
    }

    /// <summary>
    ///     Converts the provided <see cref="MqttValueProviderResult"/> into a comma-separated string containing all
    ///     submitted values.
    /// </summary>
    /// <param name="result">The <see cref="MqttValueProviderResult"/>.</param>
    public static explicit operator string(MqttValueProviderResult result)
    {
        return result.Values.ToString();
    }

    /// <summary>
    ///     Converts the provided <see cref="MqttValueProviderResult"/> into a an array of <see cref="string"/> containing
    ///     all submitted values.
    /// </summary>
    /// <param name="result">The <see cref="MqttValueProviderResult"/>.</param>
    public static explicit operator string[](MqttValueProviderResult result)
    {
        // ToArray() handles the entirely-null case and we assume individual values are never null.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return ToStringArray(result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    /// <summary>
    ///     Converts the provided <see cref="MqttValueProviderResult"/> into a an array of <see cref="string"/> containing
    ///     all submitted values.
    /// </summary>
    /// <param name="result">The <see cref="MqttValueProviderResult"/>.</param>
    public static explicit operator checked string[](MqttValueProviderResult result)
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return result.Values.ToArray();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    /// <summary>
    ///     Converts the provided <see cref="MqttValueProviderResult"/> into a an array of <see cref="string"/> containing
    ///     all submitted values.
    /// </summary>
    /// <param name="result">The <see cref="MqttValueProviderResult"/>.</param>
    /// <returns>An array containing the values</returns>
    public static string[] ToStringArray(MqttValueProviderResult result)
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return result.Values.ToArray();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    /// <summary>
    ///     Compares two <see cref="MqttValueProviderResult"/> objects for equality.
    /// </summary>
    /// <param name="x">A <see cref="MqttValueProviderResult"/>.</param>
    /// <param name="y">A <see cref="MqttValueProviderResult"/> to compare against.</param>
    /// <returns><c>true</c> if the values are equal, otherwise <c>false</c>.</returns>
    public static bool operator ==(MqttValueProviderResult x, MqttValueProviderResult y)
    {
        return x.Equals(y);
    }

    /// <summary>
    ///     Compares two <see cref="MqttValueProviderResult"/> objects for inequality.
    /// </summary>
    /// <param name="x">A <see cref="MqttValueProviderResult"/>.</param>
    /// <param name="y">A <see cref="MqttValueProviderResult"/> to compare against.</param>
    /// <returns><c>false</c> if the values are equal, otherwise <c>true</c>.</returns>
    public static bool operator !=(MqttValueProviderResult x, MqttValueProviderResult y)
    {
        return !x.Equals(y);
    }
}
