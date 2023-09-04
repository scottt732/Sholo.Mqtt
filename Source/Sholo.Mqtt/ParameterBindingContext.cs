using System;
using System.Collections.Generic;
using System.Reflection;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt;

public class ParameterBindingContext : ParametersBindingContext
{
    public IReadOnlyDictionary<string, string> TopicArguments { get; }
    public IDictionary<ParameterInfo, object> ActionArguments { get; }
    public ParameterInfo ActionParameter { get; }

    public object Value
    {
        get => ActionArguments[ActionParameter];
        set => ActionArguments[ActionParameter] = value;
    }

    public ParameterBindingContext(
        ParametersBindingContext parametersBindingContext,
        IReadOnlyDictionary<string, string> topicArguments,
        IDictionary<ParameterInfo, object> actionArguments,
        ParameterInfo actionParameter)
        : base(
            parametersBindingContext.Action,
            parametersBindingContext.TopicName,
            parametersBindingContext.TopicPatternFilter,
            parametersBindingContext.Request,
            parametersBindingContext.Logger,
            parametersBindingContext.PayloadState
        )
    {
        TopicArguments = topicArguments;
        ActionArguments = actionArguments;
        ActionParameter = actionParameter;
    }

    public bool TryConvert(string input, IMqttParameterTypeConverter explicitParameterTypeConverter, Type targetType, out object result)
    {
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

        if (input == null)
        {
            if (ActionParameter.ParameterType.IsClass)
            {
                result = default;
                return true;
            }
            else if (ActionParameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(ActionParameter.ParameterType) != null)
            {
                result = null;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        if (DefaultTypeConverters.PrimitiveTypeConverters.TryGetValue(ActionParameter.ParameterType, out var primitiveTypeConverter))
        {
            try
            {
                result = primitiveTypeConverter.Invoke(input);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        result = default;
        return false;
    }
}
