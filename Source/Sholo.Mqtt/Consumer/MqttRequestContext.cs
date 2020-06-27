using JetBrains.Annotations;
using MQTTnet;

namespace Sholo.Mqtt.Consumer
{
    [PublicAPI]
    public class MqttRequestContext<TTopicParameters> : MqttRequestContext
        where TTopicParameters : class, new()
    {
        public TTopicParameters TopicParameters { get; }

        public MqttRequestContext(MqttRequestContext context, TTopicParameters topicParameters)
            : base(context, context.ClientId)
        {
            TopicParameters = topicParameters;
        }
    }

    [PublicAPI]
    public class MqttRequestContext : MqttApplicationMessage
    {
        public string ClientId { get; set; }

        public MqttRequestContext()
        {
        }

        public MqttRequestContext(MqttApplicationMessage message, string clientId)
        {
            ContentType = message.ContentType;
            CorrelationData = message.CorrelationData;
            MessageExpiryInterval = message.MessageExpiryInterval;
            Payload = message.Payload;
            PayloadFormatIndicator = message.PayloadFormatIndicator;
            QualityOfServiceLevel = message.QualityOfServiceLevel;
            ResponseTopic = message.ResponseTopic;
            Retain = message.Retain;
            SubscriptionIdentifiers = message.SubscriptionIdentifiers;
            Topic = message.Topic;
            TopicAlias = message.TopicAlias;
            UserProperties = message.UserProperties;
            ClientId = clientId;
        }
    }
}
