using MQTTnet.Protocol;

namespace Sholo.Mqtt.Settings
{
    public class MqttMessageSettings
    {
        public string Topic { get; set; }

        public string Payload { get; set; }

        public MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }

        public bool? Retain { get; set; }
    }
}