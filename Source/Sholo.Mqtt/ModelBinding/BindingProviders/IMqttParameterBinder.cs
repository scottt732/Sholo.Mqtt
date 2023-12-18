using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

[PublicAPI]
public interface IMqttParameterBinder
{
    bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result
    );
}
