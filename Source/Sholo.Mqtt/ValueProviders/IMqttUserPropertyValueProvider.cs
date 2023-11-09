using System;

namespace Sholo.Mqtt.ValueProviders;

[PublicAPI]
public interface IMqttUserPropertyValueProvider : IMqttValueProvider<string[]>
{
    string PropertyName { get; }
    StringComparison StringComparison { get; }
}
