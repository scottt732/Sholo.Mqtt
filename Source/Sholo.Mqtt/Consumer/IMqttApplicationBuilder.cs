using System;
using JetBrains.Annotations;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.Consumer
{
    [PublicAPI]
    public interface IMqttApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; }

        IMqttApplicationBuilder Use(MqttRequestDelegate middleware);

        IMqttApplicationBuilder Map(
            string topicPattern,
            MqttRequestDelegate middleware,
            MqttQualityOfServiceLevel? qualityOfServiceLevel = null,
            bool? noLocal = null,
            bool? retainAsPublished = null,
            MqttRetainHandling? retainHandling = null);

        IMqttApplication Build();
    }
}
