using System;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.Application.Builder;

namespace Sholo.Mqtt.Middleware;

[PublicAPI]
public static class MqttApplicationBuilderExtensions
{
    public static IMqttApplicationBuilder UseMiddleware<TMqttMiddleware>(this IMqttApplicationBuilder mqttApplicationBuilder)
        where TMqttMiddleware : IMqttMiddleware
    {
        return mqttApplicationBuilder.Use(
            next =>
            {
                return async ctx =>
                {
                    var middleware = ctx.ServiceProvider.GetService<TMqttMiddleware>() ?? Activator.CreateInstance<TMqttMiddleware>();
                    return await middleware.InvokeAsync(ctx, next);
                };
            }
        );
    }

    public static IMqttApplicationBuilder UseMiddleware<TMqttMiddleware>(this IMqttApplicationBuilder mqttApplicationBuilder, TMqttMiddleware middleware)
        where TMqttMiddleware : IMqttMiddleware
    {
        return mqttApplicationBuilder.Use(
            next =>
            {
                return async ctx => await middleware.InvokeAsync(ctx, next);
            }
        );
    }
}
