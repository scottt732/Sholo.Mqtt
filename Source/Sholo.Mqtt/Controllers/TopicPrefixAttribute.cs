using System;

namespace Sholo.Mqtt.Controllers;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class TopicPrefixAttribute : Attribute
{
    public string TopicPrefix { get; }

    public TopicPrefixAttribute(string topicPrefix)
    {
        TopicPrefix = topicPrefix ?? throw new ArgumentNullException(nameof(topicPrefix));
    }
}
