using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Sholo.Mqtt.ModelBinding.Validation;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

[PublicAPI]
public interface IMqttModelBindingResult
{
    /// <summary>
    ///     Gets a value which represents the <see cref="IMqttTopicFilter" /> associated with the
    ///     request
    /// </summary>
    IMqttTopicFilter TopicFilter { get; }

    /// <summary>
    ///     Gets the object which contains the method in the <see cref="Action" />. If the action is
    ///     anonymous, this will be null.
    /// </summary>
    TypeInfo? Instance { get; }

    /// <summary>
    ///     Gets the <see cref="MethodInfo" /> associated with the request handler (Controller action, <see cref="MqttRequestDelegate" />, etc.)
    /// </summary>
    MethodInfo Action { get; }

    /// <summary>
    ///     Gets a dictionary containing the values to bind to the <see cref="Action" />'s parameters.
    ///     Configuring the values of this dictionary is the responsibility of <see cref="IMqttModelBinder" />s,
    ///     the result of which is used by <see cref="IMqttModelValidator" />s before executing the
    ///     <see cref="Action"/>.
    /// </summary>
    IReadOnlyDictionary<ParameterInfo, ParameterState> ActionArguments { get; }

    /// <summary>
    ///     Gets a value indicating whether all parameters of the action were successfully bound
    ///     and validated. The action will only execute if this returns true.
    /// </summary>
    bool Success { get; }

    Task<bool> Invoke();
}
