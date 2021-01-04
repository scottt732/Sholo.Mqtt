using JetBrains.Annotations;
using MQTTnet.Protocol;
using Sholo.Mqtt.Application;

namespace Sholo.Mqtt.ApplicationBuilder
{
    [PublicAPI]
    public interface IMqttApplicationBuilder
    {
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
