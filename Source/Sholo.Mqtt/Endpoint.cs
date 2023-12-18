using System.Reflection;
using Sholo.Mqtt.ModelBinding;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt;

[PublicAPI]
public class Endpoint : IMqttModelBindingContext
{
    public TypeInfo? Instance { get; }
    public MethodInfo Action { get; }
    public IMqttTopicFilter TopicFilter { get; }
    public MqttRequestDelegate RequestDelegate { get; }

    public Endpoint(
        TypeInfo? instance,
        MethodInfo action,
        IMqttTopicFilter topicFilter,
        MqttRequestDelegate requestDelegate)
    {
        Instance = instance;
        Action = action;
        TopicFilter = topicFilter;
        RequestDelegate = requestDelegate;
    }
}
