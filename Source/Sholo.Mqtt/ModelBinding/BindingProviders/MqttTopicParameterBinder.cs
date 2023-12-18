using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class MqttTopicParameterBinder : IMqttParameterBinder
{
    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result)
    {
        if (parameterState.TargetType == typeof(string) && parameterState.ParameterName.Equals("topic", StringComparison.Ordinal))
        {
            result = new ParameterBindingResult(MqttBindingSource.Topic, requestContext.Topic);
            return true;
        }

        result = null;
        return false;
    }
}
