// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Sholo.Mqtt.ModelBinding.Validation;

/// <summary>
///     An entry in a <see cref="MqttValidationStateDictionary"/>. Records state information to override the default
///     behavior of validation for an object.
/// </summary>
[PublicAPI]
public class MqttValidationStateEntry
{
    /// <summary>
    ///     Gets or sets the model prefix associated with the entry.
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    ///     Gets or sets a value indicating whether the associated model object should be validated.
    /// </summary>
    public bool SuppressValidation { get; set; }
}
