using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sholo.Mqtt.RequestProcessor
{
    [PublicAPI]
    public abstract class JsonMqttRequestProcessor<TMessage> : IMqttRequestProcessor
    {
        protected ILogger Logger { get; }
        protected virtual JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings();

        protected JsonMqttRequestProcessor(ILogger logger)
        {
            Logger = logger;
        }

        public Task<bool> ProcessRequest(IMqttRequestContext context)
        {
            var payload = context.GetPayloadAsString();
            var message = JsonConvert.DeserializeObject<TMessage>(payload, JsonSerializerSettings);
            var formattedMessage = JsonConvert.SerializeObject(message, JsonSerializerSettings);

            Logger.LogInformation("Processing:");
            Logger.LogInformation(formattedMessage);

            return ProcessMessage(context, message);
        }

        protected abstract Task<bool> ProcessMessage(IMqttRequestContext context, TMessage message);
    }
}
