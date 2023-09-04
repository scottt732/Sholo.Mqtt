using System;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.Old.Test.Helpers
{
    public class TestMqttRequestContext<TTopicParameters> : TestMqttRequestContext, IMqttRequestContext<TTopicParameters>
        where TTopicParameters : class, new()
    {
        public TTopicParameters TopicParameters { get; init; }
    }

    public class TestMqttRequestContext : IMqttRequestContext
    {
        public IServiceProvider ServiceProvider { get; init; }
        public string Topic { get; init; }
        public byte[] Payload { get; init; }
        public MqttQualityOfServiceLevel QualityOfServiceLevel { get; init; }
        public bool Retain { get; init; }
        public MqttUserProperty[] UserProperties { get; init; }
        public string ContentType { get; init; }
        public string ResponseTopic { get; init; }
        public MqttPayloadFormatIndicator? PayloadFormatIndicator { get; init; }
        public uint? MessageExpiryInterval { get; init; }
        public ushort? TopicAlias { get; init; }
        public byte[] CorrelationData { get; init; }
        public uint[] SubscriptionIdentifiers { get; init; }
        public string ClientId { get; init; }
    }
}
