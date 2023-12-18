// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace Sholo.Mqtt.Internal;

/// <summary>
/// <see cref="IMqttControllerActivator"/> that uses type activation to create controllers.
/// </summary>
internal class DefaultMqttControllerActivator : IMqttControllerActivator
{
    private readonly ITypeActivatorCache _typeActivatorCache;

    public DefaultMqttControllerActivator(ITypeActivatorCache typeActivatorCache)
    {
        _typeActivatorCache = typeActivatorCache ?? throw new ArgumentNullException(nameof(typeActivatorCache));
    }

    /// <inheritdoc />
    public object Create(IMqttRequestContext requestContext, Type controllerType)
    {
        ArgumentNullException.ThrowIfNull(requestContext, nameof(requestContext));

        var serviceProvider = requestContext.ServiceProvider;

        return _typeActivatorCache.CreateInstance<object>(serviceProvider, controllerType);
    }

    /// <inheritdoc />
    public void Release(IMqttRequestContext context, object controller)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(controller, nameof(controller));

        if (controller is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <inheritdoc />
    public ValueTask ReleaseAsync(IMqttRequestContext context, object controller)
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
