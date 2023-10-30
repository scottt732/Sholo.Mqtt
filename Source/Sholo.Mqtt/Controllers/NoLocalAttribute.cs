using System;

namespace Sholo.Mqtt.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class NoLocalAttribute : Attribute
{
    public bool NoLocal { get; }

    public NoLocalAttribute(bool noLocal)
    {
        NoLocal = noLocal;
    }
}
