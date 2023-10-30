using System;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Server;
using Sholo.Mqtt.Application.Provider;
using Sholo.Mqtt.Application.Provider;
using Sholo.Mqtt.Consumer;
using Sholo.Mqtt.DependencyInjection;
using Sholo.Mqtt.Internal;
using Sholo.Mqtt.DependencyInjection;
using Sholo.Mqtt.Internal;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.TypeConverters;
using Sholo.Mqtt.TypeConverters.NewtonsoftJson;

namespace Sholo.Mqtt;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    public static IMqttServiceCollection AddMqttServices<TMqttSettings>(this IServiceCollection services, string configurationName)
        where TMqttSettings : MqttSettings, new()
    {
        services.AddOptions<TMqttSettings>()
            .BindConfiguration(configurationName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var mqttSettings = sp.GetRequiredService<IOptions<TMqttSettings>>().Value;
        services.AddSingleton(sp =>
        {
            var mqttSettings = sp.GetRequiredService<IOptions<TMqttSettings>>().Value;

            var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttSettings.Host, mqttSettings.Port)
                .WithProtocolVersion(mqttSettings.MqttProtocolVersion ?? MqttProtocolVersion.V500);
            var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttSettings.Host, mqttSettings.Port)
                .WithProtocolVersion(mqttSettings.MqttProtocolVersion ?? MqttProtocolVersion.V500);

            if (mqttSettings.UseTls)
            {
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithTlsOptions(x =>
                {
                    x.WithSslProtocols(SslProtocols.None);

                    if (mqttSettings.IgnoreCertificateValidationErorrs)
                    {
                        x.WithCertificateValidationHandler(_ => true);
                    }

                    if (!string.IsNullOrEmpty(mqttSettings.ClientCertificatePrivateKey) && !string.IsNullOrEmpty(mqttSettings.ClientCertificatePublicKey))
                    {
                        x.WithClientCertificates(new[] { X509Certificate2.CreateFromPemFile(mqttSettings.ClientCertificatePublicKey, mqttSettings.ClientCertificatePrivateKey) });
                    }
                });
            }

            if (!string.IsNullOrEmpty(mqttSettings.Username) && !string.IsNullOrEmpty(mqttSettings.Password))
            {
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithCredentials(mqttSettings.Username, mqttSettings.Password);
            }

            if (mqttSettings.ClientId != null)
            {
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithClientId(mqttSettings.ClientId);
            }

            if (mqttSettings.Timeout.HasValue)
            {
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithTimeout(mqttSettings.Timeout.Value);
            }

            mqttClientOptionsBuilder = mqttSettings.KeepAliveInterval.HasValue ?
                mqttClientOptionsBuilder.WithKeepAlivePeriod(mqttSettings.KeepAliveInterval.Value) :
                mqttClientOptionsBuilder.WithNoKeepAlive();
            mqttClientOptionsBuilder = mqttSettings.KeepAliveInterval.HasValue ?
                mqttClientOptionsBuilder.WithKeepAlivePeriod(mqttSettings.KeepAliveInterval.Value) :
                mqttClientOptionsBuilder.WithNoKeepAlive();

            var lastWillAndTestamentMessage = mqttSettings.GetLastWillAndTestamentApplicationMessage();
            if (lastWillAndTestamentMessage != null)
            {
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillPayload(lastWillAndTestamentMessage.PayloadSegment);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillRetain(lastWillAndTestamentMessage.Retain);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillTopic(lastWillAndTestamentMessage.Topic);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillContentType(lastWillAndTestamentMessage.ContentType);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillCorrelationData(lastWillAndTestamentMessage.CorrelationData);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillResponseTopic(lastWillAndTestamentMessage.ResponseTopic);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillMessageExpiryInterval(lastWillAndTestamentMessage.MessageExpiryInterval);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillPayloadFormatIndicator(lastWillAndTestamentMessage.PayloadFormatIndicator);
                mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillQualityOfServiceLevel(lastWillAndTestamentMessage.QualityOfServiceLevel);

                if (lastWillAndTestamentMessage.UserProperties != null)
                {
                    foreach (var userProperty in lastWillAndTestamentMessage.UserProperties)
                    {
                        mqttClientOptionsBuilder = mqttClientOptionsBuilder.WithWillUserProperty(userProperty.Name, userProperty.Value);
                    }
                }
            }

            var mqttClientOptions = mqttClientOptionsBuilder.Build();

            return mqttClientOptions;
        });

        services.AddSingleton<MqttFactory>();
        services.AddTransient(sp =>
        {
            var factory = sp.GetRequiredService<MqttFactory>();
            var client = factory.CreateMqttClient();
            return client;
        });

        return new MqttServiceCollection(services);
    }
        return new MqttServiceCollection(services);
    }

    public static IMqttServiceCollection AddManagedMqttServices<TMqttSettings>(this IServiceCollection services, string configurationName)
        where TMqttSettings : ManagedMqttSettings, new()
    {
        services.AddMqttServices<TMqttSettings>(configurationName);
    public static IMqttServiceCollection AddManagedMqttServices<TMqttSettings>(this IServiceCollection services, string configurationName)
        where TMqttSettings : ManagedMqttSettings, new()
    {
        services.AddMqttServices<TMqttSettings>(configurationName);

        services.AddSingleton(sp =>
        {
            var managedMqttClientStorage = sp.GetService<IManagedMqttClientStorage>();
            var mqttManagedSettings = sp.GetRequiredService<IOptions<TMqttSettings>>().Value;
            var mqttClientOptions = sp.GetRequiredService<MqttClientOptions>();

            var managedMqttClientOptionsBuilder = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .WithPendingMessagesOverflowStrategy(mqttManagedSettings.PendingMessagesOverflowStrategy ?? MqttPendingMessagesOverflowStrategy.DropNewMessage)
                .WithAutoReconnectDelay(mqttManagedSettings.AutoReconnectDelay ?? TimeSpan.FromSeconds(5.0))
                .WithMaxPendingMessages(mqttManagedSettings.MaxPendingMessages ?? int.MaxValue);
            var managedMqttClientOptionsBuilder = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .WithPendingMessagesOverflowStrategy(mqttManagedSettings.PendingMessagesOverflowStrategy ?? MqttPendingMessagesOverflowStrategy.DropNewMessage)
                .WithAutoReconnectDelay(mqttManagedSettings.AutoReconnectDelay ?? TimeSpan.FromSeconds(5.0))
                .WithMaxPendingMessages(mqttManagedSettings.MaxPendingMessages ?? int.MaxValue);

            if (managedMqttClientStorage != null)
            {
                managedMqttClientOptionsBuilder = managedMqttClientOptionsBuilder.WithStorage(managedMqttClientStorage);
            }
            if (managedMqttClientStorage != null)
            {
                managedMqttClientOptionsBuilder = managedMqttClientOptionsBuilder.WithStorage(managedMqttClientStorage);
            }

            var managedMqttClientOptions = managedMqttClientOptionsBuilder.Build();
            var managedMqttClientOptions = managedMqttClientOptionsBuilder.Build();

            return managedMqttClientOptions;
        });
            return managedMqttClientOptions;
        });

        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<MqttFactory>();
            var mqttClient = factory.CreateManagedMqttClient();
            return mqttClient;
        });

        return new MqttServiceCollection(services);
    }
        return new MqttServiceCollection(services);
    }

    public static IMqttServiceCollection AddManagedMqttServices<TMqttSettings, TStorage>(this IServiceCollection services, string configurationName)
        where TMqttSettings : ManagedMqttSettings, new()
        where TStorage : class, IManagedMqttClientStorage
    {
        services.AddSingleton<IManagedMqttClientStorage, TStorage>();
        services.AddManagedMqttServices<TMqttSettings>(configurationName);
    public static IMqttServiceCollection AddManagedMqttServices<TMqttSettings, TStorage>(this IServiceCollection services, string configurationName)
        where TMqttSettings : ManagedMqttSettings, new()
        where TStorage : class, IManagedMqttClientStorage
    {
        services.AddSingleton<IManagedMqttClientStorage, TStorage>();
        services.AddManagedMqttServices<TMqttSettings>(configurationName);

        return new MqttServiceCollection(services);
    }
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
        return services;
    }
}
