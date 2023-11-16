using System.Threading.Tasks;

namespace Sholo.Mqtt.Middleware;

[PublicAPI]
public class RoutingMiddleware : IMqttMiddleware
{
    public async Task<bool> InvokeAsync(IMqttRequestContext context, MqttRequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var requestDelegate = endpoint?.RequestDelegate;

        if (requestDelegate == null)
        {
            return await next.Invoke(context);
        }

        return await requestDelegate.Invoke(context);
    }
}
