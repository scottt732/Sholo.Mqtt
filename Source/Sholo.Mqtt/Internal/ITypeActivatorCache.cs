// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.Internal;

/// <summary>
/// Caches <see cref="ObjectFactory"/> instances produced by
/// <see cref="ActivatorUtilities.CreateFactory"/>.
/// </summary>
internal interface ITypeActivatorCache
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TInstance"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve dependencies for
    /// <paramref name="optionType"/>.</param>
    /// <param name="optionType">The <see cref="Type"/> of the <typeparamref name="TInstance"/> to create.</param>
    /// <typeparam name="TInstance">The instance type to create</typeparam>
    /// <returns>A <typeparamref name="TInstance" /> instance</returns>
    TInstance CreateInstance<TInstance>(IServiceProvider serviceProvider, Type optionType);
}
