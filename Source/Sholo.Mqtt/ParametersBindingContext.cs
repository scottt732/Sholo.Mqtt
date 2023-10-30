using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.Topics.PatternFilter;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt;

public class ParametersBindingContext
{
    public MethodInfo Action { get; }
    public string TopicName { get; }
    public IMqttTopicPatternFilter TopicPatternFilter { get; }
    public MqttRequestContext Request { get; }
    public ILogger Logger { get; }
    public IMqttParameterTypeConverter[] ParameterTypeConverters => LazyParameterTypeConverters.Value;
    public PayloadState PayloadState { get; }

    private Lazy<IMqttParameterTypeConverter[]> LazyParameterTypeConverters { get; }

    public ParametersBindingContext(
        MethodInfo action,
        string topicName,
        IMqttTopicPatternFilter topicPatternFilter,
        MqttRequestContext request,
        ILogger logger,
        PayloadState payloadState
    )
    {
        Action = action;
        TopicName = topicName;
        TopicPatternFilter = topicPatternFilter;
        Request = request;
        Logger = logger;
        PayloadState = payloadState ?? new PayloadState();

        LazyParameterTypeConverters = new Lazy<IMqttParameterTypeConverter[]>(RetrieveParameterTypeConverters);
    }

    private IMqttParameterTypeConverter[] RetrieveParameterTypeConverters() =>
        Request.ServiceProvider
            .GetService<IEnumerable<IMqttParameterTypeConverter>>()
            .Reverse()
            .ToArray();
}
