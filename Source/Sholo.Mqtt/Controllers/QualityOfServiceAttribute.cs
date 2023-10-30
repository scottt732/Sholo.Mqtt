using System;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class QualityOfServiceAttribute : Attribute
{
    public MqttQualityOfServiceLevel QualityOfServiceLevel { get; }

    public QualityOfServiceAttribute(MqttQualityOfServiceLevel qualityOfServiceLevel)
    {
        QualityOfServiceLevel = qualityOfServiceLevel;
    }
}
