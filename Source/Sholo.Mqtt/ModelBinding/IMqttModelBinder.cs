using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

/*
public interface IMqttModelBinder
{
    bool CanBind(Type targetType);
}

// ReSharper disable once UnusedTypeParameter - Used to disambiguate in DI
public interface IMqttModelBinder<TSource> : IMqttModelBinder
{
}

public interface IMqttModelBinder<TSource, TTarget> : IMqttModelBinder<TSource>
{
    bool TryGetValue(TSource source, out TTarget target);
}
*/

public interface IMqttModelBinder
{
    /// <summary>
    /// Attempts to bind a model.
    /// </summary>
    /// <param name="bindingContext">The <see cref="ModelBindingContext"/>.</param>
    /// <returns>
    /// <para>
    /// A <see cref="Task"/> which will complete when the model binding process completes.
    /// </para>
    /// <para>
    /// If model binding was successful, the <see cref="ModelBindingContext.Result"/> should have
    /// <see cref="MqttModelBindingResult.IsModelSet"/> set to <c>true</c>.
    /// </para>
    /// <para>
    /// A model binder that completes successfully should set <see cref="ModelBindingContext.Result"/> to
    /// a value returned from <see cref="MqttModelBindingResult.Success"/>.
    /// </para>
    /// </returns>
    Task BindModelAsync(IMqttModelBindingContext bindingContext);
    bool TryPerformModelBinding(
        IMqttRequestContext requestContext,
        IMqttTopicFilter topicPatternFilter,
        MethodInfo action,
        [MaybeNullWhen(false)] out IDictionary<ParameterInfo, object?> actionArguments);
}

public class CancellationTokenModelBinder : IMqttModelBinder
{
    public Task BindModelAsync(IMqttModelBindingContext bindingContext)
    {
        bindingContext.Result = MqttModelBindingResult.Success(bindingContext.Request.ShutdownToken);
        throw new System.NotImplementedException();
    }

    public bool TryPerformModelBinding(IMqttRequestContext requestContext, IMqttTopicFilter topicPatternFilter, MethodInfo action, out IDictionary<ParameterInfo, object?> actionArguments)
    {
        throw new System.NotImplementedException();
    }
}

public class ServicesMqttModelBinder : IMqttModelBinder
{
    public bool TryPerformModelBinding(IMqttRequestContext requestContext, IMqttTopicFilter topicPatternFilter, MethodInfo action, out IDictionary<ParameterInfo, object?> actionArguments)
    {
        throw new System.NotImplementedException();
    }
}
