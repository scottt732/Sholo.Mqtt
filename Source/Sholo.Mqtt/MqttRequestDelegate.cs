using System.Threading.Tasks;

namespace Sholo.Mqtt
{
    public delegate Task<bool> MqttRequestDelegate(IMqttRequestContext context);

    public delegate Task<bool> MqttRequestDelegate<in TParameters>(IMqttRequestContext<TParameters> context)
        where TParameters : class, new();
}
