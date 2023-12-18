using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class MqttServiceProviderParameterBinder : IMqttParameterBinder
{
    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result)
    {
        if (parameterState.TargetType == typeof(IServiceProvider))
        {
            result = new ParameterBindingResult(MqttBindingSource.Context, requestContext.ServiceProvider, bypassValidation: true);
            return true;
        }

        result = null;
        return false;
    }
}
