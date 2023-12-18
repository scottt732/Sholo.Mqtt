using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

[PublicAPI]
public class MqttModelBindingResult : IMqttModelBindingResult
{
    public IMqttTopicFilter TopicFilter { get; }
    public TypeInfo? Instance { get; }
    public MethodInfo Action { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<ParameterInfo, ParameterState> ActionArguments { get; private set; }

    /// <inheritdoc />
    public bool Success => ActionArguments.All(x => x.Value is { IsModelSet: true, ValidationStatus: ParameterValidationResult.ValidationSuppressed or ParameterValidationResult.Valid });

    public MqttModelBindingResult(IMqttModelBindingContext modelBindingContext, IDictionary<ParameterInfo, ParameterState> actionArguments)
    {
        TopicFilter = modelBindingContext.TopicFilter;
        Instance = modelBindingContext.Instance;
        Action = modelBindingContext.Action;
        ActionArguments = new ReadOnlyDictionary<ParameterInfo, ParameterState>(actionArguments);
    }

    public Task<bool> Invoke()
    {
        if (!Success)
        {
            throw new InvalidOperationException("The model binding did not complete successfully. Invocation is impossible.");
        }

        var arguments = ActionArguments.Values.Select(x => x.Value).ToArray();

        if (Action.ReturnType == typeof(Task<bool>))
        {
            return (Task<bool>)Action.Invoke(null, arguments)!;
        }

        if (Action.ReturnType == typeof(bool))
        {
            return Task.FromResult((bool)Action.Invoke(null, arguments)!);
        }

        throw new InvalidOperationException("Expecting either a Task<bool> or a bool return type");
    }
}
