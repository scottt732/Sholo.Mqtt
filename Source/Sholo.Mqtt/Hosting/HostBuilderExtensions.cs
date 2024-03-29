using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sholo.Mqtt.Application.Builder;
using Sholo.Mqtt.Application.BuilderConfiguration;

namespace Sholo.Mqtt.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureMqttHost(this IHostBuilder builder, Action<IMqttApplicationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        builder.ConfigureServices((_, services) =>
        {
            var cfg = new ConfigureMqttApplicationBuilder(configure);
            services.AddSingleton<IConfigureMqttApplicationBuilder>(cfg);
        });

        return builder;
    }
}
