using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt;

public class PayloadBindingContext : ParametersBindingContext
{
    public IReadOnlyDictionary<string, string> TopicArguments { get; }
    public IDictionary<ParameterInfo, object> ActionArguments { get; }
    public ParameterInfo PayloadParameter { get; }
    public IMqttRequestPayloadTypeConverter PayloadTypeConverter => LazyPayloadTypeConverter.Value;

    public object Payload
    {
        get => ActionArguments[PayloadParameter];
        set => ActionArguments[PayloadParameter] = value;
    }

    private Lazy<IMqttRequestPayloadTypeConverter> LazyPayloadTypeConverter { get; }

    public PayloadBindingContext(
        ParametersBindingContext parametersBindingContext,
        IReadOnlyDictionary<string, string> topicArguments,
        IDictionary<ParameterInfo, object> actionArguments,
        ParameterInfo payloadParameter)
        : base(
            parametersBindingContext.Action,
            parametersBindingContext.TopicName,
            parametersBindingContext.TopicPatternFilter,
            parametersBindingContext.Request,
            parametersBindingContext.Logger,
            parametersBindingContext.PayloadState)
    {
        TopicArguments = topicArguments;
        ActionArguments = actionArguments;
        PayloadParameter = payloadParameter;

        LazyPayloadTypeConverter = new Lazy<IMqttRequestPayloadTypeConverter>(RetrievePayloadTypeConverter);
    }

    private IMqttRequestPayloadTypeConverter RetrievePayloadTypeConverter() =>
        Request.ServiceProvider.GetService<IMqttRequestPayloadTypeConverter>() ??
        throw new InvalidOperationException("There is no type converter registered for the MQTT payload");
}
