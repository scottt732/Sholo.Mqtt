using System;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Server;

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
                var mqttSettings = sp.GetRequiredService<IOptions<TMqttSettings>>().Value;
                var factory = sp.GetRequiredService<IMqttFactory>();
                var client = factory.CreateMqttClient();

                var onlineMessage = mqttSettings.GetOnlineApplicationMessage();
                if (onlineMessage != null)
                {
                    client.UseConnectedHandler(async args =>
                    {
                        await client.PublishAsync(onlineMessage);
                    });
                }

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
                var hostApplicationLifetime = sp.GetRequiredService<IHostApplicationLifetime>();
                var factory = sp.GetRequiredService<IMqttFactory>();
                var logger = sp.GetRequiredService<ILogger<IManagedMqttClient>>();
                var mqttSettings = sp.GetRequiredService<IOptions<TMqttSettings>>();
                var managedMqttClientOptions = sp.GetRequiredService<IManagedMqttClientOptions>();

                logger.LogInformation("Connecting to MQTT broker at {host}:{port}...", mqttSettings.Value.Host, mqttSettings.Value.Port ?? 1883);

                var mqttClient = factory.CreateManagedMqttClient();
                var onlineMessage = mqttSettings.Value.GetOnlineApplicationMessage();

                var mre = new ManualResetEventSlim();
                mqttClient.UseConnectedHandler(async ea =>
                {
                    logger.LogInformation("Connected to MQTT broker.");
                    if (onlineMessage != null)
                    {
                        logger.LogInformation("Sending online message to {topic}", onlineMessage.Topic);
                        await mqttClient.PublishAsync(onlineMessage);
                    }

                    mre.Set();
                });

                mqttClient.UseDisconnectedHandler(ea =>
                {
                    if (!hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
                    {
                        logger.LogWarning("Attempting to restore MQTT broker connection...");
                    }
                });

#pragma warning disable VSTHRD002
                mqttClient.StartAsync(managedMqttClientOptions).Wait();
#pragma warning restore VSTHRD002

                mre.Wait(hostApplicationLifetime.ApplicationStopping);

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
    }
}
