using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;

namespace Sholo.Mqtt;

[PublicAPI]
public static class MqttRequestContextExtensions
{
    public static Task PublishAsync(
        this MqttRequestContext requestContext,
        Action<MqttApplicationMessageBuilder> messageBuilder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestContext, nameof(requestContext));
        ArgumentNullException.ThrowIfNull(messageBuilder, nameof(messageBuilder));

        var builder = new MqttApplicationMessageBuilder();
        messageBuilder.Invoke(builder);
        var message = builder.Build();

        return requestContext.PublishAsync(message, cancellationToken);
    }

    public static Task RespondAsync(
        this MqttRequestContext requestContext,
        MqttApplicationMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestContext, nameof(requestContext));
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        message.Topic = requestContext.ResponseTopic;
        message.CorrelationData = requestContext.CorrelationData;

        return requestContext.PublishAsync(message, cancellationToken);
    }

    public static Task RespondAsync(
        this MqttRequestContext requestContext,
        Action<MqttApplicationMessageBuilder> messageBuilder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestContext, nameof(requestContext));
        ArgumentNullException.ThrowIfNull(messageBuilder, nameof(messageBuilder));

        var builder = new MqttApplicationMessageBuilder();
        messageBuilder.Invoke(builder);
        var message = builder.Build();

        return requestContext.RespondAsync(message, cancellationToken);
    }
}
