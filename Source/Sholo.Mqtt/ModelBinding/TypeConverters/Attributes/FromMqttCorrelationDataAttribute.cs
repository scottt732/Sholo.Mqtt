using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttCorrelationDataAttribute : BaseFromMqttConverterAttribute<IMqttCorrelationDataTypeConverter, DefaultTypeConverter>
{
    public FromMqttCorrelationDataAttribute()
        : base(MqttBindingSource.CorrelationData)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttCorrelationDataTypeConverter typeConverter, out object? result)
    {
        return typeConverter.TryConvertCorrelationData(
            requestContext.CorrelationData,
            parameterState.ParameterInfo.ParameterType,
            out result
        );
    }
}

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttCorrelationDataAttribute<TMqttCorrelationDataTypeConverter> : BaseFromMqttConverterAttribute<IMqttCorrelationDataTypeConverter, TMqttCorrelationDataTypeConverter>
    where TMqttCorrelationDataTypeConverter : class, IMqttCorrelationDataTypeConverter
{
    public FromMqttCorrelationDataAttribute()
        : base(MqttBindingSource.CorrelationData)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttCorrelationDataTypeConverter typeConverter, out object? result)
    {
        return typeConverter.TryConvertCorrelationData(
            requestContext.CorrelationData,
            parameterState.ParameterInfo.ParameterType,
            out result
        );
    }
}
