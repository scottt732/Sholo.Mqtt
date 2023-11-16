using JetBrains.Annotations;

namespace Sholo.Mqtt.Test.Specimens;

[PublicAPI]
public enum TestLightState
{
    Unknown,
    On,
    Off,
    Unavailable
}
