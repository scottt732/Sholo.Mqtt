using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sholo.Mqtt.Old.ApplicationBuilder;
using Sholo.Mqtt.Old.ApplicationBuilderConfiguration;

namespace Sholo.Mqtt.Old
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureMqttHost(this IHostBuilder builder, Action<IMqttApplicationBuilder> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.ConfigureServices(services =>
            {
                var cfg = new ConfigureMqttApplicationBuilder(configure);
                services.AddSingleton<IConfigureMqttApplicationBuilder>(cfg);
            });

            return builder;
        }
    }
}
