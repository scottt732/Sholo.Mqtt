using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Sholo.Mqtt.Controllers;

namespace Mqtt.Sample
{
    [TopicPrefix("test")]
    public class TestController : MqttControllerBase
    {
        public ILogger<TestController> Logger { get; }

        public TestController(ILogger<TestController> logger)
        {
            Logger = logger;
        }

        [Topic("run/+user/+count", "RunUser")]
        public Task<bool> Run(
            string user,
            int count,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running {User}: {Count} {Payload}", user, count, Encoding.ASCII.GetString(Request.Payload));
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(true);
        }

        [Topic("run/+user/+count/2", "RunUser")]
        public Task<bool> Run2(
            string user,
            int count,
            RunModel model,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running {User}: {Count} Hello {Payload}", user, count, model.Hello);
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(true);
        }
    }

    [PublicAPI]
    public class RunModel
    {
        public string Hello { get; set; }

        [Required]
        public string Test { get; set; }
    }
}
