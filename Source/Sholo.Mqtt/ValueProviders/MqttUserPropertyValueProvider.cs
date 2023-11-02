using System;
using System.Linq;

namespace Sholo.Mqtt.ValueProviders;

public class MqttUserPropertyValueProvider : IMqttValueProvider<string>
{
    public string PropertyName { get; }
    public StringComparison StringComparison { get; }

    public MqttUserPropertyValueProvider(string propertyName, StringComparison stringComparison = StringComparison.Ordinal)
    {
        PropertyName = propertyName;
        StringComparison = stringComparison;
    }

    public string GetValueSource(ParameterBindingContext context)
    {
        return context.Request.MqttUserProperties
            .Where(x => x.Name.Equals(PropertyName, StringComparison))
            .Select(x => x.Value)
            .FirstOrDefault();
    }
}
