using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Server;
using Sholo.Mqtt.Application.Provider;
using Sholo.Mqtt.Consumer;
using Sholo.Mqtt.DependencyInjection;
using Sholo.Mqtt.Internal;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.TypeConverters;
using Sholo.Mqtt.TypeConverters.NewtonsoftJson;
using Sholo.Mqtt.TypeConverters.Payload.NewtonsoftJson;

namespace Sholo.Mqtt;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    public static IMqttServiceCollection AddMqttServices<TMqttSettings>(this IServiceCollection services, string configurationName)
        where TMqttSettings : MqttSettings, new()
    {
        services.AddOptions<TMqttSettings>()
            .BindConfiguration(configurationName)
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

            var mqttClientOptions = mqttClientOptionsBuilder.Build();

            return mqttClientOptions;
        });

        services.AddSingleton<IMqttFactory, MqttFactory>();
        services.AddTransient(sp =>
        {
            var factory = sp.GetRequiredService<IMqttFactory>();
            var client = factory.CreateMqttClient();
            return client;
        });

        return new MqttServiceCollection(services);
    }

    public static IMqttServiceCollection AddManagedMqttServices<TMqttSettings>(this IServiceCollection services, string configurationName)
        where TMqttSettings : ManagedMqttSettings, new()
    {
        services.AddMqttServices<TMqttSettings>(configurationName);

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

        return new MqttServiceCollection(services);
    }

    public static IMqttServiceCollection AddManagedMqttServices<TMqttSettings, TStorage>(this IServiceCollection services, string configurationName)
        where TMqttSettings : ManagedMqttSettings, new()
        where TStorage : class, IManagedMqttClientStorage
    {
        services.AddSingleton<IManagedMqttClientStorage, TStorage>();
        services.AddManagedMqttServices<TMqttSettings>(configurationName);

        return new MqttServiceCollection(services);
    }

    public static IMqttServiceCollection AddMqttConsumerService<TMqttSettings>(
        this IServiceCollection services,
        string configurationName
    )
        where TMqttSettings : ManagedMqttSettings, new()
    {
        services.TryAddSingleton<ITypeActivatorCache, TypeActivatorCache>();
        services.TryAddSingleton<IControllerActivator, DefaultControllerActivator>();
        services.TryAddSingleton<IRouteProvider, RouteProvider>();

        services.TryAddSingleton<IMqttApplicationProvider, MqttApplicationProvider>();
        services.AddManagedMqttServices<TMqttSettings>(configurationName);
        services.AddHostedService<MqttConsumerService<TMqttSettings>>();

        return new MqttServiceCollection(services);
    }

    public static IMqttServiceCollection AddMqttConsumerService(
        this IServiceCollection services,
        string configurationName
    )
        => services.AddMqttConsumerService<ManagedMqttSettings>(configurationName);

    public static IMqttServiceCollection AddNewtonsoftJsonPayloadConverter(
        this IMqttServiceCollection services,
        Action<NewtonsoftJsonTypeConverterOptions> configuration = null)
    {
        services.Configure<NewtonsoftJsonTypeConverterOptions>(opt =>
        {
            configuration?.Invoke(opt);
        });

        services.TryAddSingleton<NewtonsoftJsonTypeConverter>();
        services.TryAddSingleton<IMqttRequestPayloadTypeConverter>(sp => sp.GetRequiredService<NewtonsoftJsonTypeConverter>());

        return services;
    }
}
