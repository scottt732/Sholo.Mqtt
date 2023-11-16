// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace Sholo.Mqtt.Internal;

/// <summary>
/// Provides methods to create a controller.
/// </summary>
public interface IControllerActivator
{
    /// <summary>
    /// Creates a controller.
    /// </summary>
    /// <param name="context">The <see cref="IMqttRequestContext"/> for the executing action.</param>
    /// <param name="controllerType">The controller type to create.</param>
    /// <returns>An instance of the controller type specified</returns>
    object Create(IMqttRequestContext context, Type controllerType);

    /// <summary>
    /// Releases a controller.
    /// </summary>
    /// <param name="context">The <see cref="IMqttRequestContext"/> for the executing action.</param>
    /// <param name="controller">The controller to release.</param>
    void Release(IMqttRequestContext context, object controller);

    /// <summary>
    /// Releases a controller asynchronously.
    /// </summary>
    /// <param name="context">The <see cref="IMqttRequestContext"/> for the executing action.</param>
    /// <param name="controller">The controller to release.</param>
    /// <returns>A <see cref="ValueTask" /> for the disposal</returns>
    ValueTask ReleaseAsync(IMqttRequestContext context, object controller);
}
