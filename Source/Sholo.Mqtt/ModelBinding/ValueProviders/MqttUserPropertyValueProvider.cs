using System;
using System.Linq;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

public class MqttUserPropertyValueProvider : IMqttUserPropertyValueProvider
{
    public string PropertyName { get; }
    public StringComparison StringComparison { get; }

    public MqttUserPropertyValueProvider(string propertyName, StringComparison stringComparison = StringComparison.Ordinal)
    {
        PropertyName = propertyName;
        StringComparison = stringComparison;
    }

    public string[] GetValueSource(IMqttModelBindingContext mqttModelBindingContext)
    {
        return mqttModelBindingContext.Request.UserProperties
            .Where(x => x.Name.Equals(PropertyName, StringComparison))
            .Select(x => x.Value)
            .ToArray();
    }
}
