using System.Threading.Tasks;

namespace Sholo.Mqtt.Application.Builder
{
    public class RoutingMiddleware : IMqttMiddleware
    {
        public async Task<bool> InvokeAsync(MqttRequestContext context, MqttRequestDelegate next)
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
}
