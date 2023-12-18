using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttPayloadAttribute : BaseFromMqttConverterAttribute<IMqttPayloadTypeConverter, DefaultTypeConverter>
{
    public FromMqttPayloadAttribute()
        : base(MqttBindingSource.Payload)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttPayloadTypeConverter typeConverter, out object? result)
    {
        return typeConverter.TryConvertPayload(
            requestContext.Payload,
            parameterState.ParameterInfo.ParameterType,
            out result
        );
    }
}

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttPayloadAttribute<TMqttPayloadTypeConverter> : BaseFromMqttConverterAttribute<IMqttPayloadTypeConverter, TMqttPayloadTypeConverter>
    where TMqttPayloadTypeConverter : class, IMqttPayloadTypeConverter
{
    public FromMqttPayloadAttribute(string? serviceKey = null)
        : base(MqttBindingSource.Payload, serviceKey)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttPayloadTypeConverter typeConverter, out object? result)
    {
        return typeConverter.TryConvertPayload(
            requestContext.Payload,
            parameterState.ParameterInfo.ParameterType,
            out result
        );
    }
}
