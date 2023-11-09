using System.Threading.Tasks;
using Sholo.Mqtt.ModelBinding.Context;

namespace Sholo.Mqtt.Application.Builder;

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
