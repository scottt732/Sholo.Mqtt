using System;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Sholo.Mqtt
{
    public interface IMqttRequestContext
    {
        IServiceProvider ServiceProvider { get; }
        string Topic { get; }
        byte[] Payload { get; }
        MqttQualityOfServiceLevel QualityOfServiceLevel { get; }
        bool Retain { get; }
        MqttUserProperty[] UserProperties { get; }
        string ContentType { get; }
        string ResponseTopic { get; }
        MqttPayloadFormatIndicator? PayloadFormatIndicator { get; }
        uint? MessageExpiryInterval { get; }
        ushort? TopicAlias { get; }
        byte[] CorrelationData { get; }
        uint[] SubscriptionIdentifiers { get; }
        string ClientId { get; }
    }

    public interface IMqttRequestContext<out TTopicParameters> : IMqttRequestContext
        where TTopicParameters : class, new()
    {
        TTopicParameters TopicParameters { get; }
    }
}
