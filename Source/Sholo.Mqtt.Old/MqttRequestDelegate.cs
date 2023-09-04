using System.Threading.Tasks;

namespace Sholo.Mqtt.Old
{
    public delegate Task<bool> MqttRequestDelegate(IMqttRequestContext context);

    public delegate Task<bool> MqttRequestDelegate<in TTopicParameters>(IMqttRequestContext<TTopicParameters> context)
        where TTopicParameters : class;
}
