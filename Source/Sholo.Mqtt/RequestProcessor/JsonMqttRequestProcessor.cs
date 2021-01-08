using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Sholo.Mqtt.RequestProcessor
{
    [PublicAPI]
    public abstract class JsonMqttRequestProcessor<TMessage> : IMqttRequestProcessor
    {
        protected virtual JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings();

        public Task<bool> ProcessRequest(IMqttRequestContext context)
        {
            var payload = context.GetPayloadAsString();
            var message = JsonConvert.DeserializeObject<TMessage>(payload, JsonSerializerSettings);

            return ProcessMessage(context, message);
        }

        protected abstract Task<bool> ProcessMessage(IMqttRequestContext context, TMessage message);
    }
}
