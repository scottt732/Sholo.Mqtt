using System.Threading.Tasks;

namespace Sholo.Mqtt.Old
{
    public delegate Task<bool> TypedMqttRequestDelegate<in TPayload>(ITypedMqttRequestContext<TPayload> context)
        where TPayload : class;

    public delegate Task<bool> TypedMqttRequestDelegate<in TParameters, in TPayload>(ITypedMqttRequestContext<TParameters, TPayload> context)
        where TParameters : class
        where TPayload : class;
}
