using System.Threading.Tasks;

namespace Sholo.Mqtt.Middleware;

public interface IMqttMiddleware
{
    /// <summary>
    /// Request handling method.
    /// </summary>
    /// <param name="context">The <see cref="IMqttRequestContext"/> for the current request.</param>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
#pragma warning disable CA1716
    Task<bool> InvokeAsync(IMqttRequestContext context, MqttRequestDelegate next);
#pragma warning restore CA1716
}
