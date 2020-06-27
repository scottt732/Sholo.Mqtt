using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.Consumer
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttConsumerService(
            this IServiceCollection services,
            IConfiguration mqttConfiguration = null,
            Action<IMqttApplicationBuilder> configurator = null
        )
            => services.AddMqttConsumerService<ManagedMqttSettings>(
                mqttConfiguration,
                configurator
            );

        public static IServiceCollection AddMqttConsumerService<TMqttSettings>(
            this IServiceCollection services,
            IConfiguration mqttConfiguration = null,
            Action<IMqttApplicationBuilder> configurator = null
        )
            where TMqttSettings : ManagedMqttSettings, new()
        {
            services.AddManagedMqttServices<TMqttSettings>(mqttConfiguration);

            services.AddSingleton(sp =>
            {
                var configurators = sp.GetService<IEnumerable<IConfigureMqttApplicationBuilder>>();
                var appBuilder = new MqttApplicationBuilder(sp);

                if (configurators != null)
                {
                    foreach (var cfg in configurators)
                    {
                        cfg.Configure(appBuilder);
                    }
                }

                configurator?.Invoke(appBuilder);

                var app = appBuilder.Build();
                return app;
            });

            services.AddHostedService<MqttConsumerService<TMqttSettings>>();

            return services;
        }
    }
}
