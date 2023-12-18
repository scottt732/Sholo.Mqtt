// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

/// <summary>
/// A context that contains operating information for model binding and validation.
/// </summary>
/// <remarks>
///     This is largely copy-and-paste from https://github.com/dotnet/aspnetcore
/// </remarks>
[PublicAPI]
public interface IMqttModelBindingContext
{
    /// <summary>
    ///     Gets a value which represents the <see cref="IMqttTopicFilter" /> associated with the
    ///     request
    /// </summary>
    IMqttTopicFilter TopicFilter { get; }

    /// <summary>
    ///     Gets the object which contains the method in the <see cref="Action" />. If the action is
    ///     anonymous, this will be null.
    /// </summary>
    TypeInfo? Instance { get; }

    /// <summary>
    ///     Gets the <see cref="MethodInfo" /> associated with the request handler (Controller action, <see cref="MqttRequestDelegate" />, etc.)
    /// </summary>
    MethodInfo Action { get; }
}
