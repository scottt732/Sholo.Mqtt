using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using Sholo.Mqtt.ModelBinding.TypeConverters;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class MqttTopicArgumentParameterBinder : IMqttParameterBinder
{
    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result)
    {
        if (topicArguments.TryGetValue(parameterState.ParameterName, out var stringValues) &&
            DefaultTypeConverter.Instance.TryConvertTopicArgument(stringValues!, parameterState.ParameterInfo.ParameterType, out var resultObj))
        {
            result = new ParameterBindingResult(MqttBindingSource.Topic, resultObj);
            return true;
        }

        result = null;
        return false;
    }
}
