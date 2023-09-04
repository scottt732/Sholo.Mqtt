using System;

namespace Sholo.Mqtt.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class TopicAttribute : Attribute
    {
        public string TopicPattern { get; }
        public string Name { get; }

        public TopicAttribute(
            string topicPattern,
            string name = null)
        {
            TopicPattern = topicPattern;
            Name = name;
        }
    }
}
