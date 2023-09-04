using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sholo.Mqtt.Application.Builder;
using Sholo.Mqtt.Application.BuilderConfiguration;

namespace Sholo.Mqtt.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureMqttHost(this IHostBuilder builder, Action<IMqttApplicationBuilder> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.ConfigureServices((ctx, services) =>
            {
                var cfg = new ConfigureMqttApplicationBuilder(configure);
                services.AddSingleton<IConfigureMqttApplicationBuilder>(cfg);
            });

            return builder;
        }
    }
}
