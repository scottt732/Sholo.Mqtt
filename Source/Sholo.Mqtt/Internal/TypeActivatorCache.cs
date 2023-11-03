// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.Internal;

/// <summary>
/// Caches <see cref="ObjectFactory"/> instances produced by
/// <see cref="ActivatorUtilities.CreateFactory(Type, Type[])"/>.
/// </summary>
internal class TypeActivatorCache : ITypeActivatorCache
{
    private readonly Func<Type, ObjectFactory> _createFactory = (type) => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
    private readonly ConcurrentDictionary<Type, ObjectFactory> _typeActivatorCache = new();

    /// <inheritdoc/>
    public TInstance CreateInstance<TInstance>(
        IServiceProvider serviceProvider,
        Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(implementationType, nameof(implementationType));

        var createFactory = _typeActivatorCache.GetOrAdd(implementationType, _createFactory);
        return (TInstance)createFactory(serviceProvider, arguments: null);
    }
}
