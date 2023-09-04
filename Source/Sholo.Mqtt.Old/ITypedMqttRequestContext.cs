namespace Sholo.Mqtt.Old
{
    public interface ITypedMqttRequestContext<out TPayload>
        : IMqttRequestContext
    {
        TPayload TypedPayload { get; }
    }

    public interface ITypedMqttRequestContext<out TTopicParameters, out TPayload> :
        IMqttRequestContext<TTopicParameters>,
        ITypedMqttRequestContext<TPayload>
            where TTopicParameters : class
    {
    }
}
