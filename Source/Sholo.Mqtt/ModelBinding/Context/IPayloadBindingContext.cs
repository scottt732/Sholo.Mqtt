using System.Collections.Generic;
using System.Reflection;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt.ModelBinding.Context;

[PublicAPI]
public interface IPayloadBindingContext : IParametersBindingContext
{
    IDictionary<ParameterInfo, object?> ActionArguments { get; }
    ParameterInfo PayloadParameter { get; }
    IMqttRequestPayloadTypeConverter PayloadTypeConverter { get; }
}
