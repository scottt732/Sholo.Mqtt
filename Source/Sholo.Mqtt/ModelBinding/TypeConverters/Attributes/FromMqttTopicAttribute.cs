using System;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttTopicAttribute : BaseFromMqttConverterAttribute<IMqttTopicArgumentTypeConverter, DefaultTypeConverter>
{
    public FromMqttTopicAttribute()
        : base(MqttBindingSource.Topic)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttTopicArgumentTypeConverter typeConverter, out object? result)
    {
        return typeConverter.TryConvertTopicArgument(
            parameterState.ParameterName,
            parameterState.ParameterInfo.ParameterType,
            out result
        );
    }
}

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMqttTopicAttribute<TMqttTopicArgumentTypeConverter> : BaseFromMqttConverterAttribute<IMqttTopicArgumentTypeConverter, TMqttTopicArgumentTypeConverter>
    where TMqttTopicArgumentTypeConverter : class, IMqttTopicArgumentTypeConverter
{
    public FromMqttTopicAttribute()
        : base(MqttBindingSource.Topic)
    {
    }

    protected override bool TryConvert(IMqttRequestContext requestContext, ParameterState parameterState, IMqttTopicArgumentTypeConverter typeConverter, out object? result)
    {
        return typeConverter.TryConvertTopicArgument(
            parameterState.ParameterName,
            parameterState.ParameterInfo.ParameterType,
            out result
        );
    }
}
