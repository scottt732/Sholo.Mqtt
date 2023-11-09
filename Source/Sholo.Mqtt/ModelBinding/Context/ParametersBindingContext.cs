using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt.ModelBinding.Context;

public class ParametersBindingContext : IParametersBindingContext
{
    public MethodInfo Action { get; }
    public string TopicName { get; }
    public IReadOnlyDictionary<string, string[]> TopicArguments { get; }
    public IMqttRequestContext Request { get; }
    public ILogger? Logger { get; }
    public IMqttParameterTypeConverter[] ParameterTypeConverters => LazyParameterTypeConverters.Value;

    private Lazy<IMqttParameterTypeConverter[]> LazyParameterTypeConverters { get; }

    public ParametersBindingContext(
        MethodInfo action,
        string topicName,
        IMqttRequestContext request,
        IReadOnlyDictionary<string, string[]> topicArguments,
        ILogger? logger
    )
    {
        Action = action;
        TopicName = topicName;
        Request = request;
        Logger = logger;
        TopicArguments = topicArguments;

        LazyParameterTypeConverters = new Lazy<IMqttParameterTypeConverter[]>(RetrieveParameterTypeConverters);
    }

    private IMqttParameterTypeConverter[] RetrieveParameterTypeConverters() =>
        Request.ServiceProvider
            .GetService<IEnumerable<IMqttParameterTypeConverter>>()
            ?.Reverse()
            .ToArray()
        ?? Array.Empty<IMqttParameterTypeConverter>();
}
