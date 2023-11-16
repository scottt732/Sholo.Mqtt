using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Sholo.Mqtt;

[PublicAPI]
internal class MqttRequestContext : IMqttRequestContext
{
    public IServiceProvider ServiceProvider { get; }
    public string Topic { get; } = null!;
    public ArraySegment<byte> Payload { get; }
    public MqttQualityOfServiceLevel QualityOfServiceLevel { get; }
    public bool Retain { get; }
    public MqttUserProperty[] UserProperties { get; } = null!;
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

    private Lazy<IManagedMqttClient> ClientFactory { get; }
    private Lazy<IHostApplicationLifetime?> HostApplicationLifetimeFactory { get; }
    private Lazy<CancellationToken> ShutdownTokenFactory { get; }

    private IManagedMqttClient Client => ClientFactory.Value;

    public MqttRequestContext(MqttRequestContext context)
        : this(
            context.ServiceProvider,
            context.Topic,
            context.Payload,
            context.QualityOfServiceLevel,
            context.Retain,
            context.UserProperties,
            context.ContentType,
            context.Dup,
            context.ResponseTopic,
            context.PayloadFormatIndicator,
            context.MessageExpiryInterval,
            context.TopicAlias,
            context.CorrelationData,
            context.SubscriptionIdentifiers,
            context.ClientId
        )
    {
    }

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
            clientId
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
        string clientId
    )
        : this(serviceProvider)
    {
        Topic = topic;
        Payload = payload;
        QualityOfServiceLevel = qualityOfServiceLevel;
        Retain = retain;
        UserProperties = userProperties ?? Array.Empty<MqttUserProperty>();
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
