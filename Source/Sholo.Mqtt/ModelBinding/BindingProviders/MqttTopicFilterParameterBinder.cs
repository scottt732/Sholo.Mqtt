using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class MqttTopicFilterParameterBinder : IMqttParameterBinder
{
    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result)
    {
        if (parameterState.TargetType == typeof(IMqttTopicFilter))
        {
            result = new ParameterBindingResult(MqttBindingSource.Context, modelBindingContext.TopicFilter, bypassValidation: true);
            return true;
        }

        result = null;
        return false;
    }
}
