using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sholo.Mqtt.Middleware;

[PublicAPI]
public class LoggingMiddleware : IMqttMiddleware
{
    private ILogger<LoggingMiddleware> Logger { get; }

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        Logger = logger;
    }

    public async Task<bool> InvokeAsync(IMqttRequestContext context, MqttRequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await next.Invoke(context);

            Logger.LogDebug(
                "Executed in {Duration:F0}ms",
                stopwatch.ElapsedMilliseconds
            );

            return result;
        }
        catch (Exception exc)
        {
            Logger.LogError(
                exc,
                "Error executing after {Duration:F0}ms: {Message}",
                stopwatch.Elapsed,
                exc.Message
            );

            throw;
        }
    }
}
