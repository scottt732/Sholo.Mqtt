using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sholo.Mqtt.ModelBinding;

internal static class MqttRequestContextExtensions
{
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static MqttRequestDelegate GetRequestDelegate(this IMqttRequestContext context, object? instance)
    {
        return ctx =>
        {
            if (!ReferenceEquals(ctx, context))
            {
                throw new InvalidOperationException($"This ${nameof(MqttRequestDelegate)} was created from a different {nameof(IMqttRequestContext)}");
            }

            if (ctx.ModelBindingResult is not { Success: true })
            {
                throw new InvalidOperationException("The model binding did not complete successfully. Invocation is impossible.");
            }

            var arguments = ctx.ModelBindingResult.ActionArguments.Values.Select(x => x.Value).ToArray();

            if (ctx.ModelBindingResult.Action.ReturnType == typeof(Task<bool>))
            {
                return (Task<bool>)ctx.ModelBindingResult.Action.Invoke(instance, arguments)!;
            }

            if (ctx.ModelBindingResult.Action.ReturnType == typeof(ValueTask<bool>))
            {
                return ((ValueTask<bool>)ctx.ModelBindingResult.Action.Invoke(instance, arguments)!).AsTask();
            }

            if (ctx.ModelBindingResult.Action.ReturnType == typeof(bool))
            {
                return Task.FromResult((bool)ctx.ModelBindingResult.Action.Invoke(instance, arguments)!);
            }

            throw new InvalidOperationException("Expecting action to have a Task<bool>, ValueTask<bool>, or a bool return type");
        };
    }
}
