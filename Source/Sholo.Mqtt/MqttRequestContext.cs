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
public class MqttRequestContext
{
    internal Endpoint GetEndpoint()
    {
        var routeProvider = ServiceProvider.GetRequiredService<IRouteProvider>();
        return routeProvider.GetEndpoint(this);
    }

    public IServiceProvider ServiceProvider { get; }
    public string Topic { get; }
    public ArraySegment<byte> Payload { get; }
    public MqttQualityOfServiceLevel QualityOfServiceLevel { get; }
    public bool Retain { get; }
    public MqttUserProperty[] MqttUserProperties { get; }
    public string ContentType { get; }
    public string ResponseTopic { get; }
    public MqttPayloadFormatIndicator? PayloadFormatIndicator { get; }
    public uint? MessageExpiryInterval { get; }
    public ushort? TopicAlias { get; }
    public byte[] CorrelationData { get; }
    public uint[] SubscriptionIdentifiers { get; }
    public string ClientId { get; }
    public CancellationToken ShutdownToken => ShutdownTokenFactory.Value;

    private Lazy<IManagedMqttClient> ClientFactory { get; }
    private Lazy<IHostApplicationLifetime> HostApplicationLifetimeFactory { get; }
    private Lazy<CancellationToken> ShutdownTokenFactory { get; }

    private IManagedMqttClient Client => ClientFactory.Value;

    public MqttRequestContext(MqttRequestContext context)
        : this()
    {
        ServiceProvider = context.ServiceProvider;
        Topic = context.Topic;
        Payload = context.Payload;
        QualityOfServiceLevel = context.QualityOfServiceLevel;
        Retain = context.Retain;
        MqttUserProperties = context.MqttUserProperties;
        ContentType = context.ContentType;
        ResponseTopic = context.ResponseTopic;
        PayloadFormatIndicator = context.PayloadFormatIndicator;
        MessageExpiryInterval = context.MessageExpiryInterval;
        TopicAlias = context.TopicAlias;
        CorrelationData = context.CorrelationData;
        SubscriptionIdentifiers = context.SubscriptionIdentifiers;
        ClientId = context.ClientId;
    }

    internal MqttRequestContext(
        IServiceProvider serviceProvider,
        string topic,
        byte[] payload,
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        bool retain,
        MqttUserProperty[] userProperties,
        string contentType,
        string responseTopic,
        MqttPayloadFormatIndicator payloadFormatIndicator,
        uint? messageExpiryInterval,
        ushort? topicAlias,
        byte[] correlationData,
        uint[] subscriptionIdentifiers,
        string clientId
    )
        : this()
    {
        ServiceProvider = serviceProvider;
        Topic = topic;
        Payload = payload;
        QualityOfServiceLevel = qualityOfServiceLevel;
        Retain = retain;
        MqttUserProperties = userProperties ?? Array.Empty<MqttUserProperty>();
        ContentType = contentType;
        ResponseTopic = responseTopic;
        PayloadFormatIndicator = payloadFormatIndicator;
        MessageExpiryInterval = messageExpiryInterval;
        TopicAlias = topicAlias;
        CorrelationData = correlationData;
        SubscriptionIdentifiers = subscriptionIdentifiers ?? Array.Empty<uint>();
        ClientId = clientId;
    }

    internal MqttRequestContext(IServiceProvider serviceProvider, MqttApplicationMessage message, string clientId)
        : this()
    {
        ServiceProvider = serviceProvider;
        Topic = message.Topic;
        Payload = message.PayloadSegment;
        QualityOfServiceLevel = message.QualityOfServiceLevel;
        Retain = message.Retain;
        MqttUserProperties = message.UserProperties?.ToArray() ?? Array.Empty<MqttUserProperty>();
        ContentType = message.ContentType;
        ResponseTopic = message.ResponseTopic;
        PayloadFormatIndicator = message.PayloadFormatIndicator;
        MessageExpiryInterval = message.MessageExpiryInterval;
        TopicAlias = message.TopicAlias;
        CorrelationData = message.CorrelationData;
        SubscriptionIdentifiers = message.SubscriptionIdentifiers?.ToArray() ?? Array.Empty<uint>();
        ClientId = clientId;
    }

    private MqttRequestContext()
    {
        ClientFactory = new Lazy<IManagedMqttClient>(() => ServiceProvider.GetRequiredService<IManagedMqttClient>());
        HostApplicationLifetimeFactory = new Lazy<IHostApplicationLifetime>(() => ServiceProvider.GetService<IHostApplicationLifetime>());
        ShutdownTokenFactory = new Lazy<CancellationToken>(() => HostApplicationLifetimeFactory.Value?.ApplicationStopping ?? default);
    }

    public Task PublishAsync(
        MqttApplicationMessage message,
        CancellationToken cancellationToken = default)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return Client.EnqueueAsync(message);
    }
}
