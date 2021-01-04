using System;
using System.Linq;
using JetBrains.Annotations;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Sholo.Mqtt
{
    [PublicAPI]
    public class MqttRequestContext<TTopicParameters> : MqttRequestContext, IMqttRequestContext<TTopicParameters>
        where TTopicParameters : class, new()
    {
        public TTopicParameters TopicParameters { get; }

        public MqttRequestContext(IMqttRequestContext context, TTopicParameters topicParameters)
            : base(context)
        {
            TopicParameters = topicParameters;
        }
    }

    [PublicAPI]
    public class MqttRequestContext : IMqttRequestContext
    {
        public IServiceProvider ServiceProvider { get; }
        public string Topic { get; }
        public byte[] Payload { get; }
        public MqttQualityOfServiceLevel QualityOfServiceLevel { get; }
        public bool Retain { get; }
        public MqttUserProperty[] UserProperties { get; }
        public string ContentType { get; }
        public string ResponseTopic { get; }
        public MqttPayloadFormatIndicator? PayloadFormatIndicator { get; }
        public uint? MessageExpiryInterval { get; }
        public ushort? TopicAlias { get; }
        public byte[] CorrelationData { get; }
        public uint[] SubscriptionIdentifiers { get; }
        public string ClientId { get; }

        public MqttRequestContext(
            IServiceProvider serviceProvider,
            string topic,
            byte[] payload,
            MqttQualityOfServiceLevel qualityOfServiceLevel,
            bool retain,
            MqttUserProperty[] userProperties,
            string contentType,
            string responseTopic,
            MqttPayloadFormatIndicator? payloadFormatIndicator,
            uint? messageExpiryInterval,
            ushort? topicAlias,
            byte[] correlationData,
            uint[] subscriptionIdentifiers,
            string clientId)
        {
            ServiceProvider = serviceProvider;
            Topic = topic;
            Payload = payload;
            QualityOfServiceLevel = qualityOfServiceLevel;
            Retain = retain;
            UserProperties = userProperties;
            ContentType = contentType;
            ResponseTopic = responseTopic;
            PayloadFormatIndicator = payloadFormatIndicator;
            MessageExpiryInterval = messageExpiryInterval;
            TopicAlias = topicAlias;
            CorrelationData = correlationData;
            SubscriptionIdentifiers = subscriptionIdentifiers;
            ClientId = clientId;
        }

        public MqttRequestContext(IMqttRequestContext context)
        {
            ServiceProvider = context.ServiceProvider;
            ContentType = context.ContentType;
            CorrelationData = context.CorrelationData;
            MessageExpiryInterval = context.MessageExpiryInterval;
            Payload = context.Payload;
            PayloadFormatIndicator = context.PayloadFormatIndicator;
            QualityOfServiceLevel = context.QualityOfServiceLevel;
            ResponseTopic = context.ResponseTopic;
            Retain = context.Retain;
            SubscriptionIdentifiers = context.SubscriptionIdentifiers?.ToArray() ?? Array.Empty<uint>();
            Topic = context.Topic;
            TopicAlias = context.TopicAlias;
            UserProperties = context.UserProperties?.ToArray() ?? Array.Empty<MqttUserProperty>();
            ClientId = context.ClientId;
        }

        public MqttRequestContext(IServiceProvider serviceProvider, MqttApplicationMessage message, string clientId)
        {
            ServiceProvider = serviceProvider;
            ContentType = message.ContentType;
            CorrelationData = message.CorrelationData;
            MessageExpiryInterval = message.MessageExpiryInterval;
            Payload = message.Payload;
            PayloadFormatIndicator = message.PayloadFormatIndicator;
            QualityOfServiceLevel = message.QualityOfServiceLevel;
            ResponseTopic = message.ResponseTopic;
            Retain = message.Retain;
            SubscriptionIdentifiers = message.SubscriptionIdentifiers?.ToArray() ?? Array.Empty<uint>();
            Topic = message.Topic;
            TopicAlias = message.TopicAlias;
            UserProperties = message.UserProperties?.ToArray() ?? Array.Empty<MqttUserProperty>();
            ClientId = clientId;
        }
    }
}
