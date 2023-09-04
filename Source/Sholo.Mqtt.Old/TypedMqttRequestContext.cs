using System;
using JetBrains.Annotations;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.Old
{
    [PublicAPI]
    public class TypedMqttRequestContext<TPayload> : MqttRequestContext, ITypedMqttRequestContext<TPayload>
    {
        public TPayload TypedPayload { get; }

        public TypedMqttRequestContext(
            IServiceProvider serviceProvider,
            string topic,
            byte[] payload,
            TPayload typedPayload,
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
            : base(
                serviceProvider,
                topic,
                payload,
                qualityOfServiceLevel,
                retain,
                userProperties,
                contentType,
                responseTopic,
                payloadFormatIndicator,
                messageExpiryInterval,
                topicAlias,
                correlationData,
                subscriptionIdentifiers,
                clientId)
        {
            TypedPayload = typedPayload;
        }

        public TypedMqttRequestContext(
            IMqttRequestContext context,
            TPayload typedPayload)
            : base(context)
        {
            TypedPayload = typedPayload;
        }

        public TypedMqttRequestContext(
            IServiceProvider serviceProvider,
            MqttApplicationMessage message,
            string clientId,
            TPayload typedPayload)
            : base(
                serviceProvider,
                message,
                clientId)
        {
            TypedPayload = typedPayload;
        }
    }

    [PublicAPI]
    public class TypedMqttRequestContext<TTopicParameters, TPayload> : TypedMqttRequestContext<TPayload>, ITypedMqttRequestContext<TTopicParameters, TPayload>
        where TTopicParameters : class
    {
        public TTopicParameters TopicParameters { get; }

        public TypedMqttRequestContext(
            IServiceProvider serviceProvider,
            string topic,
            TTopicParameters topicParameters,
            byte[] payload,
            TPayload typedPayload,
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
            : base(
                serviceProvider,
                topic,
                payload,
                typedPayload,
                qualityOfServiceLevel,
                retain,
                userProperties,
                contentType,
                responseTopic,
                payloadFormatIndicator,
                messageExpiryInterval,
                topicAlias,
                correlationData,
                subscriptionIdentifiers,
                clientId)
        {
            TopicParameters = topicParameters;
        }

        public TypedMqttRequestContext(
            IMqttRequestContext context,
            TTopicParameters topicParameters,
            TPayload typedPayload)
            : base(
                context,
                typedPayload)
        {
            TopicParameters = topicParameters;
        }

        public TypedMqttRequestContext(
            IServiceProvider serviceProvider,
            MqttApplicationMessage message,
            string clientId,
            TTopicParameters topicParameters,
            TPayload typedPayload)
            : base(
                serviceProvider,
                message,
                clientId,
                typedPayload)
        {
            TopicParameters = topicParameters;
        }
    }
}
