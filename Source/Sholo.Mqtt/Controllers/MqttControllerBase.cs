namespace Sholo.Mqtt.Controllers;

[MqttController]
public abstract class MqttControllerBase
{
    public IMqttRequestContext Request { get; internal set; } = null!;
}
