using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class MqttAttributeParameterBinder : IMqttParameterBinder
{
    public bool TryBind(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments,
        ParameterState parameterState,
        [MaybeNullWhen(false)] out ParameterBindingResult result)
    {
        var fromMqttConverters = parameterState.ParameterInfo.GetCustomAttributes(false).OfType<IFromMqttConverter>();

        foreach (var fromMqttConverter in fromMqttConverters)
        {
            if (fromMqttConverter.TryBind(requestContext, parameterState, out var resultObj))
            {
                result = new ParameterBindingResult(fromMqttConverter.BindingSource, resultObj);
                return true;
            }
        }

        result = null;
        return false;
    }
}
