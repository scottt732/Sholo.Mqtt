using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt.ModelBinding.Context;

[PublicAPI]
public interface IParametersBindingContext
{
    MethodInfo Action { get; }
    string TopicName { get; }

    IReadOnlyDictionary<string, string[]> TopicArguments { get; }
    IMqttRequestContext Request { get; }
    ILogger? Logger { get; }
    IMqttParameterTypeConverter[] ParameterTypeConverters { get; }
}
