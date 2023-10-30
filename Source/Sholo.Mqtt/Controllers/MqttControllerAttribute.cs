using System;

namespace Sholo.Mqtt.Controllers;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class MqttControllerAttribute : Attribute
{
}
