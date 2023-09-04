using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.Application.Builder
{
    [PublicAPI]
    public static class MqttApplicationBuilderExtensions
    {
        public static IMqttApplicationBuilder UseRouting(this IMqttApplicationBuilder mqttApplicationBuilder)
        {
            return mqttApplicationBuilder.UseMiddleware<RoutingMiddleware>();
        }

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
}
