using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Sholo.Mqtt.Consumer;

namespace Sholo.Mqtt.Test.Processors.Test
{
    public class TestProcessor : IMqttRequestProcessor<TestParameters>
    {
        private ILogger Logger { get; }

        public TestProcessor(ILogger<TestProcessor> logger)
        {
            Logger = logger;
        }

        public Task<bool> ProcessRequest(MqttRequestContext<TestParameters> context)
        {
            Logger.LogInformation($"{context.TopicParameters.One}");
            Logger.LogInformation($"{context.TopicParameters.Two}");
            Logger.LogInformation($"{context.TopicParameters.Three}");
            Logger.LogInformation(context.ConvertPayloadToString());

            return Task.FromResult(true);
        }
    }
}
