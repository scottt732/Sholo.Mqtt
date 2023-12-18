using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class MqttCancellationTokenParameterBinder : IMqttParameterBinder
{
    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result)
    {
        if (parameterState.TargetType == typeof(CancellationToken))
        {
            result = new ParameterBindingResult(MqttBindingSource.Context, requestContext.ShutdownToken, bypassValidation: true);
            return true;
        }

        result = null;
        return false;
    }
}
