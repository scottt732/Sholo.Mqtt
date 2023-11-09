#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt.ModelBinding.Context;

public class ParameterBindingContext : ParametersBindingContext, IParameterBindingContext
{
    public IDictionary<ParameterInfo, object?> ActionArguments { get; }
    public ParameterInfo ActionParameter { get; }

    public object? Value
    {
        get => ActionArguments[ActionParameter];
        set => ActionArguments[ActionParameter] = value;
    }

    public ParameterBindingContext(
        IParametersBindingContext parametersBindingContext,
        IDictionary<ParameterInfo, object?> actionArguments,
        ParameterInfo actionParameter)
        : base(
            parametersBindingContext.Action,
            parametersBindingContext.TopicName,
            parametersBindingContext.Request,
            parametersBindingContext.TopicArguments,
            parametersBindingContext.Logger
        )
    {
        ActionArguments = actionArguments;
        ActionParameter = actionParameter;
    }

    public bool TryConvert(string? input, IMqttParameterTypeConverter? explicitParameterTypeConverter, Type targetType, out object? result)
    {
        if (input == null)
        {
            if (ActionParameter.ParameterType.IsClass)
            {
                result = default;
                return true;
            }

            if (ActionParameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(ActionParameter.ParameterType) != null)
            {
                result = null;
                return true;
            }

            result = null;
            return false;
        }

        if (explicitParameterTypeConverter != null)
        {
            if (explicitParameterTypeConverter.TryConvert(input, targetType, out result))
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException(
                    $"The converter {explicitParameterTypeConverter.GetType().Name} cannot convert parameters of type {ActionParameter.ParameterType.Name}");
            }
        }

        foreach (var converter in ParameterTypeConverters)
        {
            if (converter.TryConvert(input, targetType, out result))
            {
                return true;
            }
        }

        if (DefaultTypeConverters.TryConvert(input, ActionParameter.ParameterType, out result))
        {
            return true;
        }

        result = default;
        return false;
    }
}
