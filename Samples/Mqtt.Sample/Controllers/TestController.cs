using System.Text;
using Microsoft.Extensions.Logging;
using Mqtt.Sample.Models;
using Sholo.Mqtt.Controllers;

namespace Mqtt.Sample.Controllers;

[TopicPrefix("test")]
public class TestController : MqttControllerBase
{
    private ILogger<TestController> Logger { get; }

    public TestController(ILogger<TestController> logger)
    {
        Logger = logger;
    }

    [Topic("run/+user/+count", "RunUser")]
    public Task<bool> RunAsync(
        string user,
        int count,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Running {User}: {Count} {Payload}", user, count, Encoding.ASCII.GetString(Request.Payload));
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(true);
    }

    [Topic("run/+user/+count/2", "RunUser")]
    public Task<bool> Run2Async(
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
