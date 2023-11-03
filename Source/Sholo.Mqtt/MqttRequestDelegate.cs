using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sholo.Mqtt;

/// <summary>
/// A function that can process an MQTT request.
/// </summary>
/// <param name="context">The <see cref="MqttRequestContext"/> for the request.</param>
/// <returns>A task that represents the completion of request processing.</returns>
[SuppressMessage("Microsoft.Reliability", "CA1711", Justification = "Ignored")]
public delegate Task<bool> MqttRequestDelegate(MqttRequestContext context);
