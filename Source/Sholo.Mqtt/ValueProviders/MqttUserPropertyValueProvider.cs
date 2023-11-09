#nullable enable

using System;
using System.Linq;
using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.ValueProviders;

public class MqttUserPropertyValueProvider : IMqttUserPropertyValueProvider
{
    public string PropertyName { get; }
    public StringComparison StringComparison { get; }

    public MqttUserPropertyValueProvider(string propertyName, StringComparison stringComparison = StringComparison.Ordinal)
    {
        PropertyName = propertyName;
        StringComparison = stringComparison;
    }

    public string[] GetValueSource(IParameterBindingContext context)
    {
        return context.Request.MqttUserProperties
            .Where(x => x.Name.Equals(PropertyName, StringComparison))
            .Select(x => x.Value)
            .ToArray();
    }
}
