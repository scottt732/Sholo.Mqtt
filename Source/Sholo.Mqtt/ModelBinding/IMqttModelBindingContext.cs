// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Primitives;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using Sholo.Mqtt.ModelBinding.Validation;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

/// <summary>
/// A context that contains operating information for model binding and validation.
/// </summary>
/// <remarks>
///     This is largely copy-and-paste from https://github.com/dotnet/aspnetcore
/// </remarks>
[PublicAPI]
public interface IMqttModelBindingContext : IServiceProvider
{
    /// <summary>
    ///     Gets a value which represents the <see cref="MqttBindingSource"/> associated with the
    ///     request
    /// </summary>
    MqttBindingSource? BindingSource { get; }

    /// <summary>
    ///     Gets the <see cref="IMqttRequestContext"/> associated with this context.
    /// </summary>
    IMqttRequestContext Request { get; }

    /// <summary>
    ///     Gets a value which represents the <see cref="IMqttTopicFilter" /> associated with the
    ///     request
    /// </summary>
    IMqttTopicFilter TopicFilter { get; }

    /// <summary>
    ///     Gets a value which represents the arguments extracted from the <see cref="TopicFilter" />
    /// </summary>
    IReadOnlyDictionary<string, StringValues> TopicArguments { get; }

    /// <summary>
    ///     Gets the <see cref="MethodInfo" /> associated with the request handler (Controller action, <see cref="MqttRequestDelegate" />, etc.)
    /// </summary>
    MethodInfo Action { get; }

    /// <summary>
    ///     Gets a dictionary containing the values to bind to the <see cref="Action" />'s parameters.
    ///     Configuring the values of this dictionary is the responsibility of <see cref="IMqttModelBinder" />s,
    ///     the result of which is used by <see cref="IMqttModelValidator" />s before executing the
    ///     <see cref="Action"/>.
    /// </summary>
    IReadOnlyDictionary<ParameterInfo, ParameterState> ActionArguments { get; }

    // Parameters binding
    IMqttParameterTypeConverter[] ParameterTypeConverters { get; }

    // Correlation Data
    IMqttCorrelationDataTypeConverter CorrelationDataTypeConverter { get; }

    // Payload binding
    IMqttPayloadTypeConverter PayloadTypeConverter { get; }

    bool TryConvertParameter(string? input, IMqttParameterTypeConverter? explicitParameterTypeConverter, ParameterInfo actionParameter, Type targetType, out object? result);
}
