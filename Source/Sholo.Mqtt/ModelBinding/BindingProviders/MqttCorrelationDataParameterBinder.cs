using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class MqttCorrelationDataParameterBinder : IMqttParameterBinder
{
    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result)
    {
        if (parameterState.TargetType == typeof(byte[]) && parameterState.ParameterName.Equals("correlationData", StringComparison.Ordinal))
        {
            result = new ParameterBindingResult(MqttBindingSource.CorrelationData, requestContext.CorrelationData);
            return true;
        }

        if (parameterState.TargetType == typeof(ArraySegment<byte>) && parameterState.ParameterName.Equals("correlationData", StringComparison.Ordinal))
        {
            result = new ParameterBindingResult(MqttBindingSource.CorrelationData, requestContext.CorrelationData);
            return true;
        }

        result = null;
        return false;
    }
}
