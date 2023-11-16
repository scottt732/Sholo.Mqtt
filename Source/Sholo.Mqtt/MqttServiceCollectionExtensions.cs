using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sholo.Mqtt.DependencyInjection;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using Sholo.Mqtt.ModelBinding.TypeConverters.Json;

namespace Sholo.Mqtt;

[PublicAPI]
public static class MqttServiceCollectionExtensions
{
    public static IMqttServiceCollection AddJsonPayloadConverter(
        this IMqttServiceCollection services,
        Action<IServiceProvider, JsonTypeConverterOptions>? configuration = null)
    {
        services.AddOptions<JsonTypeConverterOptions>()
            .BindConfiguration($"{services.ConfigSectionPath}:TypeConverters:Json")
            .Configure<IServiceProvider>((opt, sp) => { configuration?.Invoke(sp, opt); })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddSingleton<JsonTypeConverter>();
        services.TryAddSingleton<IMqttPayloadTypeConverter>(sp => sp.GetRequiredService<JsonTypeConverter>());

        return services;
    }
}
