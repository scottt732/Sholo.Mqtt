using System.Reflection;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

public class MqttModelBindingContext : IMqttModelBindingContext
{
    public IMqttTopicFilter TopicFilter { get; }
    public TypeInfo? Instance { get; }
    public MethodInfo Action { get; }

    public MqttModelBindingContext(
        TypeInfo? instance,
        MethodInfo action,
        IMqttTopicFilter topicFilter
    )
    {
        Instance = instance;
        Action = action;
        TopicFilter = topicFilter;
    }
}
