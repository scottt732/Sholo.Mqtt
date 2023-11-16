using System;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

[PublicAPI]
public interface IMqttUserPropertyValueProvider
{
    string PropertyName { get; }
    StringComparison StringComparison { get; }
}
