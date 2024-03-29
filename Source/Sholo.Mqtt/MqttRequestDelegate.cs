using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sholo.Mqtt;

// TODO: CancellationToken

/// <summary>
/// A function that can process an MQTT request.
/// </summary>
/// <param name="context">The <see cref="IMqttRequestContext"/> representing the incoming request.</param>
/// <returns>A task that represents the completion of request processing.</returns>
[SuppressMessage("Microsoft.Reliability", "CA1711", Justification = "Ignored")]
public delegate Task<bool> MqttRequestDelegate(IMqttRequestContext context);
