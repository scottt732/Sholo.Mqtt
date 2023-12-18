using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.Middleware;

namespace Sholo.Mqtt.Routing;

[PublicAPI]
public class RoutingMiddleware : IMqttMiddleware
{
    public async Task<bool> InvokeAsync(IMqttRequestContext context, MqttRequestDelegate next)
    {
        var routeProvider = context.ServiceProvider.GetService<IRouteProvider>();
        var endpoint = routeProvider?.GetEndpoint(context);
        var requestDelegate = endpoint?.RequestDelegate;

        if (requestDelegate == null)
        {
            return await next.Invoke(context);
        }

        return await requestDelegate.Invoke(context);
    }
}
