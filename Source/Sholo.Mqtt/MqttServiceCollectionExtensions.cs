using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sholo.Mqtt.DependencyInjection;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt;

[PublicAPI]
public static class MqttServiceCollectionExtensions
{
    public static IMqttServiceCollection AddJsonPayloadConverter(
        this IMqttServiceCollection services,
        Action<JsonTypeConverterOptions>? configuration = null)
    {
        services.AddOptions<JsonTypeConverterOptions>()
            .BindConfiguration($"{services.ConfigSectionPath}:TypeConverters:Json")
            .Configure(opt => { configuration?.Invoke(opt); })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddSingleton<JsonTypeConverter>();
        services.TryAddSingleton<IMqttRequestPayloadTypeConverter>(sp => sp.GetRequiredService<JsonTypeConverter>());

        return services;
    }
}
