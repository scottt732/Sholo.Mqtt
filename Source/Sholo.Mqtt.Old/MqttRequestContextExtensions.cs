using System;
using System.Text;

namespace Sholo.Mqtt.Old
{
    public static class MqttRequestContextExtensions
    {
        public static string GetPayloadAsString(this IMqttRequestContext requestContext, Encoding encoding = null)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }

            if (requestContext.Payload == null)
            {
                return null;
            }

            if (requestContext.Payload.Length == 0)
            {
                return string.Empty;
            }

            var useEncoding = encoding ?? Encoding.UTF8;

            return useEncoding.GetString(requestContext.Payload, 0, requestContext.Payload.Length);
        }
    }
}
