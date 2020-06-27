using System.Threading.Tasks;

namespace Sholo.Mqtt.Consumer
{
    public delegate Task<bool> MqttRequestDelegate(MqttRequestContext context);

    public delegate Task<bool> MqttRequestDelegate<TParameters>(MqttRequestContext<TParameters> context)
        where TParameters : class, new();
}