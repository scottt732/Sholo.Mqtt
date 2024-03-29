﻿using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sholo.Mqtt.DependencyInjection;
using Sholo.Mqtt.ModelBinding.TypeConverters;

namespace Sholo.Mqtt.TypeConverters.NewtonsoftJson;

[PublicAPI]
public static class MqttServiceCollectionExtensions
{
    public static IMqttServiceCollection AddNewtonsoftJsonTypeConverter(
        this IMqttServiceCollection services,
        Action<NewtonsoftJsonTypeConverterOptions> configuration = null)
    {
        services.AddOptions<NewtonsoftJsonTypeConverterOptions>()
            .BindConfiguration($"{services.ConfigSectionPath}:TypeConverters:NewtonsoftJson")
            .Configure(opt => { configuration?.Invoke(opt); })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddSingleton<NewtonsoftJsonPayloadTypeConverter>();
        services.TryAddSingleton<IMqttPayloadTypeConverter>(sp => sp.GetRequiredService<NewtonsoftJsonPayloadTypeConverter>());

        return services;
    }
}
