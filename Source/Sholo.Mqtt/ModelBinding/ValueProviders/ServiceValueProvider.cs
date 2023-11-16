using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.ModelBinding.ValueProviders;

[PublicAPI]
public class ServiceValueProvider : IMqttValueProvider
{
    public virtual bool TryGetValue(IMqttModelBindingContext mqttModelBindingContext, ParameterInfo actionParameter, out object? value)
    {
        var parameterType = actionParameter.ParameterType;

        if (parameterType.IsEnum ||
            parameterType.IsPrimitive ||
            parameterType == typeof(string) ||
            Nullable.GetUnderlyingType(parameterType) != null)
        {
            value = null;
            return false;
        }

        // TODO: test with IEnumerable. Do we want Last?

        var argumentValues = mqttModelBindingContext.Request.ServiceProvider
            .GetServices(parameterType)
            .ToArray();

        if (argumentValues.Length == 0)
        {
            value = null;
            return false;
        }

        value = argumentValues.Last();
        return true;
    }
}
