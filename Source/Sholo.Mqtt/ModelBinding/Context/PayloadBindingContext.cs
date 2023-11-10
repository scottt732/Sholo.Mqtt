using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.TypeConverters;
using Sholo.Mqtt.TypeConverters.Payload;

namespace Sholo.Mqtt.ModelBinding.Context;

public class PayloadBindingContext : ParametersBindingContext, IPayloadBindingContext
{
    public IDictionary<ParameterInfo, object?> ActionArguments { get; }
    public ParameterInfo PayloadParameter { get; }
    public IMqttRequestPayloadTypeConverter PayloadTypeConverter => LazyPayloadTypeConverter.Value;

    private Lazy<IMqttRequestPayloadTypeConverter> LazyPayloadTypeConverter { get; }

    public PayloadBindingContext(
        IParametersBindingContext parametersBindingContext,
        IDictionary<ParameterInfo, object?> actionArguments,
        ParameterInfo payloadParameter)
        : base(
            parametersBindingContext.Action,
            parametersBindingContext.TopicName,
            parametersBindingContext.Request,
            parametersBindingContext.TopicArguments,
            parametersBindingContext.Logger
        )
    {
        ActionArguments = actionArguments;
        PayloadParameter = payloadParameter;

        LazyPayloadTypeConverter = new Lazy<IMqttRequestPayloadTypeConverter>(RetrievePayloadTypeConverter);
    }

    private IMqttRequestPayloadTypeConverter RetrievePayloadTypeConverter()
    {
        return Request.ServiceProvider.GetService<IMqttRequestPayloadTypeConverter>() ?? new DefaultPayloadTypeConverter();
    }
}
