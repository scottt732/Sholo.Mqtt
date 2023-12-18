using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttUserPropertiesAttribute : BaseFromMqttConverterAttribute<IMqttUserPropertiesTypeConverter, DefaultTypeConverter>
{
    public FromMqttUserPropertiesAttribute()
        : base(MqttBindingSource.UserProperties)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttUserPropertiesTypeConverter typeConverter, out object? result)
    {
        if (requestContext.UserProperties.TryGetValue(parameterState.ParameterName, out var stringValues))
        {
            typeConverter.TryConvertUserPropertyValues(stringValues, parameterState.TargetType, out var resultList);
            result = resultList;
            return true;
        }

        result = null;
        return false;
    }
}

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttUserPropertiesAttribute<TMqttUserPropertiesTypeConverter> : BaseFromMqttConverterAttribute<IMqttUserPropertiesTypeConverter, TMqttUserPropertiesTypeConverter>
    where TMqttUserPropertiesTypeConverter : class, IMqttUserPropertiesTypeConverter
{
    public FromMqttUserPropertiesAttribute()
        : base(MqttBindingSource.UserProperties)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttUserPropertiesTypeConverter typeConverter, out object? result)
    {
        if (requestContext.UserProperties.TryGetValue(parameterState.ParameterName, out var stringValues))
        {
            typeConverter.TryConvertUserPropertyValues(stringValues, parameterState.TargetType, out var resultList);
            result = resultList;
            return true;
        }

        result = null;
        return false;
    }
}
