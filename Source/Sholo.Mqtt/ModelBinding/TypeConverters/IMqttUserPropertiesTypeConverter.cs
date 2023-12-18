using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.TypeConverters;

[PublicAPI]
public interface IMqttUserPropertiesTypeConverter : IMqttTypeConverter
{
    bool TryConvertUserPropertyValues(StringValues? values, Type targetType, out IList<object?>? result);
}
