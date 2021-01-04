using System;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Server;
using Sholo.Mqtt.ApplicationBuilder;
using Sholo.Mqtt.ApplicationBuilderConfiguration;
using Sholo.Mqtt.ApplicationProvider;
using Sholo.Mqtt.Consumer;
using Sholo.Mqtt.Settings;

namespace Sholo.Mqtt
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttServices(this IServiceCollection services, IConfiguration mqttConfiguration)
            => services.AddMqttServices<MqttSettings>(mqttConfiguration);

        public static IServiceCollection AddMqttServices<TMqttSettings>(this IServiceCollection services, IConfiguration mqttConfiguration)
            where TMqttSettings : MqttSettings, new()
        {
            services.AddOptions<TMqttSettings>()
                .Bind(mqttConfiguration)
                .ValidateDataAnnotations();

            services.AddSingleton(sp =>
            {
                var mqttSettings = sp.GetRequiredService<IOptions<TMqttSettings>>().Value;

                var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
                    .WithTcpServer(mqttSettings.Host, mqttSettings.Port)
                    .WithProtocolVersion(mqttSettings.MqttProtocolVersion ?? MqttProtocolVersion.V500);

                if (mqttSettings.ClientId != null)
                {
                    mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithClientId(mqttSettings.ClientId);
                }

                if (mqttSettings.CommunicationTimeout.HasValue)
                {
                    mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithCommunicationTimeout(mqttSettings.CommunicationTimeout.Value);
                }

                mqttClientOptionsBuilder = mqttSettings.KeepAliveInterval.HasValue ?
                    mqttClientOptionsBuilder.WithKeepAlivePeriod(mqttSettings.KeepAliveInterval.Value) :
                    mqttClientOptionsBuilder.WithNoKeepAlive();

                var lastWillAndTestamentMessage = mqttSettings.GetLastWillAndTestamentApplicationMessage();
                if (lastWillAndTestamentMessage != null)
                {
                    mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillMessage(lastWillAndTestamentMessage);
                }

                return mqttClientOptionsBuilder.Build();
            });

            services.AddSingleton<IMqttFactory, MqttFactory>();
            services.AddTransient(sp =>
            {
                var factory = sp.GetRequiredService<IMqttFactory>();
                var client = factory.CreateMqttClient();
                return client;
            });

            return services;
        }

        public static IServiceCollection AddManagedMqttServices<TMqttSettings>(this IServiceCollection services, IConfiguration mqttConfiguration)
            where TMqttSettings : ManagedMqttSettings, new()
        {
            services.AddMqttServices<TMqttSettings>(mqttConfiguration);

            services.AddSingleton<IManagedMqttClientOptions>(sp =>
            {
                var managedMqttClientStorage = sp.GetService<IManagedMqttClientStorage>();
                var mqttManagedSettings = sp.GetRequiredService<IOptions<TMqttSettings>>().Value;
                var mqttClientOptions = sp.GetRequiredService<IMqttClientOptions>();

                var managedMqttClientOptionsBuilder = new ManagedMqttClientOptionsBuilder()
                    .WithClientOptions(mqttClientOptions)
                    .WithPendingMessagesOverflowStrategy(mqttManagedSettings.PendingMessagesOverflowStrategy ?? MqttPendingMessagesOverflowStrategy.DropNewMessage)
                    .WithAutoReconnectDelay(mqttManagedSettings.AutoReconnectDelay ?? TimeSpan.FromSeconds(5.0))
                    .WithMaxPendingMessages(mqttManagedSettings.MaxPendingMessages ?? int.MaxValue);

                if (managedMqttClientStorage != null)
                {
                    managedMqttClientOptionsBuilder = managedMqttClientOptionsBuilder.WithStorage(managedMqttClientStorage);
                }

                var managedMqttClientOptions = managedMqttClientOptionsBuilder.Build();

                return managedMqttClientOptions;
            });

            services.AddSingleton(sp =>
            {
                var factory = sp.GetRequiredService<IMqttFactory>();
                var mqttClient = factory.CreateManagedMqttClient();
                return mqttClient;
            });

            return services;
        }

        public static IServiceCollection AddManagedMqttServices<TMqttSettings, TStorage>(this IServiceCollection services, IConfiguration mqttConfiguration)
            where TMqttSettings : ManagedMqttSettings, new()
            where TStorage : class, IManagedMqttClientStorage
        {
            services.AddSingleton<IManagedMqttClientStorage, TStorage>();
            services.AddManagedMqttServices<TMqttSettings>(mqttConfiguration);

            return services;
        }

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
            services.TryAddSingleton<IMqttApplicationProvider, MqttApplicationProvider>();
            services.AddManagedMqttServices<TMqttSettings>(mqttConfiguration);
            services.AddSingleton<IConfigureMqttApplicationBuilder>(sp => new ConfigureMqttApplicationBuilder(configurator));
            services.AddHostedService<MqttConsumerService<TMqttSettings>>();

            return services;
        }
    }
}
