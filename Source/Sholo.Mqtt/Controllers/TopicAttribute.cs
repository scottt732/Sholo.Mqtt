using System;

namespace Sholo.Mqtt.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class TopicAttribute : Attribute
{
    public string TopicPattern { get; }

    public TopicAttribute(string topicPattern)
    {
        TopicPattern = topicPattern;
    }
}
