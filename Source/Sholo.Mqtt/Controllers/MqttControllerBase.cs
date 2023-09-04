namespace Sholo.Mqtt.Controllers
{
    [MqttController]
    public abstract class MqttControllerBase
    {
        public MqttRequestContext Request { get; internal set; }
    }
}
