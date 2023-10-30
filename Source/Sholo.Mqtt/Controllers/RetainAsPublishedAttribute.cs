using System;

namespace Sholo.Mqtt.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RetainAsPublishedAttribute : Attribute
{
    public bool RetainAsPublished { get; }

    public RetainAsPublishedAttribute(bool retainAsPublished)
    {
        RetainAsPublished = retainAsPublished;
    }
}
