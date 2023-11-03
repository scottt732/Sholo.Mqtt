// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace Sholo.Mqtt.Internal;

/// <summary>
/// <see cref="IControllerActivator"/> that uses type activation to create controllers.
/// </summary>
internal class DefaultControllerActivator : IControllerActivator
{
    private readonly ITypeActivatorCache _typeActivatorCache;

    public DefaultControllerActivator(ITypeActivatorCache typeActivatorCache)
    {
        _typeActivatorCache = typeActivatorCache ?? throw new ArgumentNullException(nameof(typeActivatorCache));
    }

    /// <inheritdoc />
    public object Create(MqttRequestContext controllerContext, Type controllerType)
    {
        ArgumentNullException.ThrowIfNull(controllerContext, nameof(controllerContext));

        var serviceProvider = controllerContext.ServiceProvider;

        return _typeActivatorCache.CreateInstance<object>(serviceProvider, controllerType);
    }

    /// <inheritdoc />
    public void Release(MqttRequestContext context, object controller)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(controller, nameof(controller));

        if (controller is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public ValueTask ReleaseAsync(MqttRequestContext context, object controller)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(controller, nameof(controller));

        if (controller is IAsyncDisposable asyncDisposable)
        {
            return asyncDisposable.DisposeAsync();
        }

        Release(context, controller);
        return default;
    }
}
