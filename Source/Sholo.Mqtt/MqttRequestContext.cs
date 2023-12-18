using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Sholo.Mqtt.ModelBinding;

namespace Sholo.Mqtt;

[PublicAPI]
internal class MqttRequestContext : IMqttRequestContext
{
    public IServiceProvider ServiceProvider { get; }
    public string Topic { get; } = null!;
    public ArraySegment<byte> Payload { get; }
    public MqttQualityOfServiceLevel QualityOfServiceLevel { get; }
    public bool Retain { get; }
    public IReadOnlyDictionary<string, StringValues> UserProperties { get; } = null!;
    public string? ContentType { get; }
    public bool Dup { get; }
    public string? ResponseTopic { get; }
    public MqttPayloadFormatIndicator? PayloadFormatIndicator { get; }
    public uint? MessageExpiryInterval { get; }
    public ushort? TopicAlias { get; }
    public byte[]? CorrelationData { get; }
    public uint[]? SubscriptionIdentifiers { get; }
    public string ClientId { get; } = null!;
    public CancellationToken ShutdownToken => ShutdownTokenFactory.Value;
    public IMqttModelBindingResult? ModelBindingResult { get; set; }

    private Lazy<IManagedMqttClient> ClientFactory { get; }
    private Lazy<IHostApplicationLifetime?> HostApplicationLifetimeFactory { get; }
    private Lazy<CancellationToken> ShutdownTokenFactory { get; }

    private IManagedMqttClient Client => ClientFactory.Value;

    internal MqttRequestContext(IServiceProvider serviceProvider, MqttApplicationMessage message, string clientId)
        : this(
            serviceProvider,
            message.Topic,
            message.PayloadSegment,
            message.QualityOfServiceLevel,
            message.Retain,
            message.UserProperties?.ToArray(),
            message.ContentType,
            message.Dup,
            message.ResponseTopic,
            message.PayloadFormatIndicator,
            message.MessageExpiryInterval,
            message.TopicAlias,
            message.CorrelationData,
            message.SubscriptionIdentifiers?.ToArray(),
            clientId,
            StringComparer.Ordinal // TODO: Configuration
        )
    {
    }

    internal MqttRequestContext(
        IServiceProvider serviceProvider,
        string topic,
        ArraySegment<byte> payload,
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        bool retain,
        MqttUserProperty[]? userProperties,
        string? contentType,
        bool dup,
        string? responseTopic,
        MqttPayloadFormatIndicator? payloadFormatIndicator,
        uint? messageExpiryInterval,
        ushort? topicAlias,
        byte[]? correlationData,
        uint[]? subscriptionIdentifiers,
        string clientId,
        IEqualityComparer<string>? userPropertiesKeyEqualityComparer)
        : this(serviceProvider)
    {
        Topic = topic;
        Payload = payload;
        QualityOfServiceLevel = qualityOfServiceLevel;
        Retain = retain;

        userProperties ??= Array.Empty<MqttUserProperty>();
        userPropertiesKeyEqualityComparer ??= StringComparer.Ordinal;
        UserProperties = new ReadOnlyDictionary<string, StringValues>(
            userProperties
                .GroupBy(
                    x => x.Name,
                    userPropertiesKeyEqualityComparer
                )
                .ToDictionary(
                    x => x.Key,
                    x => new StringValues(x.Select(y => y.Value).ToArray()),
                    userPropertiesKeyEqualityComparer
                )
        );

        ContentType = contentType;
        Dup = dup;
        ResponseTopic = responseTopic;
        PayloadFormatIndicator = payloadFormatIndicator;
        MessageExpiryInterval = messageExpiryInterval;
        TopicAlias = topicAlias;
        CorrelationData = correlationData;
        SubscriptionIdentifiers = subscriptionIdentifiers;
        ClientId = clientId;
    }

    private MqttRequestContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        ClientFactory = new Lazy<IManagedMqttClient>(serviceProvider.GetRequiredService<IManagedMqttClient>);
        HostApplicationLifetimeFactory = new Lazy<IHostApplicationLifetime?>(serviceProvider.GetService<IHostApplicationLifetime>);
        ShutdownTokenFactory = new Lazy<CancellationToken>(() => HostApplicationLifetimeFactory.Value?.ApplicationStopping ?? default);
    }

    public Task PublishAsync(
        MqttApplicationMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        return Client.EnqueueAsync(message);
    }
}
