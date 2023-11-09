#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Sholo.Mqtt.ModelBinding.Context;

/// <summary>
///     This interface is exposed to code that consumes MQTT messages either via middleware or controllers. It is a superset
///     of the <see cref="MqttApplicationMessage" /> class, and provides a request-scoped <see cref="IServiceProvider" /> for
///     service resolution, a <see cref="CancellationToken"/> which will indicates if the application is shutting down, and
///     a set of helper methods for publishing messages back to the broker in response to the incoming message (with or without
///     correlation data).
/// </summary>
[PublicAPI]
public interface IMqttRequestContext
{
    /// <summary>
    ///     Gets the MQTT topic.
    ///     In MQTT, the word topic refers to an UTF-8 string that the broker uses to filter messages for each connected
    ///     client.
    ///     The topic consists of one or more topic levels. Each topic level is separated by a forward slash (topic level
    ///     separator).
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Gets the topic alias.
    ///     Topic aliases were introduced are a mechanism for reducing the size of published packets by reducing the size of
    ///     the topic field.
    ///     A value of 0 indicates no topic alias is used.
    /// </summary>
    /// <remarks>
    ///     MQTT 5 feature only.
    /// </remarks>
    ushort? TopicAlias { get; }

    /// <summary>
    /// Gets the payload.
    /// The payload is the data bytes sent via the MQTT protocol.
    /// </summary>
    ArraySegment<byte> Payload { get; }

    /// <summary>
    ///     Gets the quality of service level.
    ///     The Quality of Service (QoS) level is an agreement between the sender of a message and the receiver of a message
    ///     that defines the guarantee of delivery for a specific message.
    ///     <list type="bullet">
    ///         <item>At most once  (0): Message gets delivered no time, once or multiple times.</item>
    ///         <item>At least once (1): Message gets delivered at least once (one time or more often).</item>
    ///         <item>Exactly once  (2): Message gets delivered exactly once (It's ensured that the message only comes once).</item>
    ///     </list>
    /// </summary>
    MqttQualityOfServiceLevel QualityOfServiceLevel { get; }

    /// <summary>
    ///     Gets a value indicating whether the message should be retained or not.
    ///     A retained message is a normal MQTT message with the retained flag set to true.
    ///     The broker stores the last retained message and the corresponding QoS for that topic.
    /// </summary>
    bool Retain { get; }

    /// <summary>
    ///     Gets the user properties.
    ///     In MQTT 5, user properties are basic UTF-8 string key-value pairs that you can append to almost every type of MQTT
    ///     packet.
    ///     As long as you don’t exceed the maximum message size, you can use an unlimited number of user properties to add
    ///     metadata to MQTT messages and pass information between publisher, broker, and subscriber.
    ///     The feature is very similar to the HTTP header concept.
    /// </summary>
    /// <remarks>
    ///     MQTT 5 feature only.
    /// </remarks>
    MqttUserProperty[] MqttUserProperties { get; }

    /// <summary>
    ///     Gets the content type.
    ///     The content type must be a UTF-8 encoded string. The content type value identifies the kind of UTF-8 encoded
    ///     payload.
    /// </summary>
    string? ContentType { get; }

    /// <summary>
    ///     Gets a value indicating whether the Client or Server has attempted to send this MQTT PUBLISH Packet.
    ///     <list type="bullet">
    ///         <item>
    ///             If <em>false</em>, it indicates that this is the first occasion that the Client or Server has attempted
    ///             to send this MQTT PUBLISH Packet.
    ///         </item>
    ///         <item>
    ///             If <em>true</em>, it indicates that this might be re-delivery of an earlier attempt to send the Packet.
    ///             The DUP flag MUST be set to 1 by the Client or Server when it attempts to re-deliver a PUBLISH Packet
    ///         </item>
    ///     </list>
    /// </summary>
    public bool Dup { get; }

    /// <summary>
    ///     Gets the response topic.
    ///     In MQTT 5 the ability to publish a response topic was added in the publish message which allows you to implement
    ///     the request/response pattern between clients that is common in web applications.
    /// </summary>
    /// <remarks>
    ///     MQTT 5 feature only.
    /// </remarks>
    string? ResponseTopic { get; }

    /// <summary>
    ///     Gets the payload format indicator.
    ///     The payload format indicator is part of any MQTT packet that can contain a payload. The indicator is an optional
    ///     byte value.
    ///     <list type="bullet">(
    ///         <item>A value of <see cref="MqttPayloadFormatIndicator.Unspecified"/> (0) indicates an unspecified byte stream (default).</item>
    ///         <item>A value of <see cref="MqttPayloadFormatIndicator.CharacterData"/> (1) indicates a UTF-8 encoded string payload.</item>
    ///     </list>
    /// </summary>
    /// <remarks>
    ///     MQTT 5 feature only.
    /// </remarks>
    MqttPayloadFormatIndicator? PayloadFormatIndicator { get; }

    /// <summary>
    ///     Gets the message expiry interval.
    ///     A client can set the message expiry interval in seconds for each PUBLISH message individually.
    ///     This interval defines the period of time that the broker stores the PUBLISH message for any matching subscribers
    ///     that are not currently connected.
    ///     When no message expiry interval is set, the broker must store the message for matching subscribers indefinitely.
    ///     When the retained=true option is set on the PUBLISH message, this interval also defines how long a message is
    ///     retained on a topic.
    /// </summary>
    /// <remarks>
    ///     MQTT 5 feature only.
    /// </remarks>
    uint? MessageExpiryInterval { get; }

    /// <summary>
    ///     Gets the correlation data.
    ///     In order for the sender to know what sent message the response refers to it can also send correlation data with the
    ///     published message.
    /// </summary>
    /// <remarks>
    ///     MQTT 5 feature only.
    /// </remarks>
    byte[]? CorrelationData { get; }

    /// <summary>
    ///     Gets the subscription identifiers.
    ///     The client can specify a subscription identifier when subscribing.
    ///     The broker will establish and store the mapping relationship between this subscription and subscription identifier
    ///     when successfully create or modify subscription.
    ///     The broker will return the subscription identifier associated with this PUBLISH packet and the PUBLISH packet to
    ///     the client when need to forward PUBLISH packets matching this subscription to this client.
    /// </summary>
    /// <remarks>
    ///     MQTT 5 feature only.
    /// </remarks>
    uint[]? SubscriptionIdentifiers { get; }

    /// <summary>
    ///     Gets an <see cref="IServiceProvider" /> scoped to the processing of this particular request
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    ///     Gets the client ID of the connection. This was either supplied by the client or generated by the broker at connection time.
    /// </summary>
    string ClientId { get; }

    /// <summary>
    ///     Gets a <see cref="CancellationToken" /> which is triggered when <see cref="IHostedService.StopAsync(CancellationToken)" />
    ///     is called
    /// </summary>
    CancellationToken ShutdownToken { get; }

    /// <summary>
    ///     Publishes a message using the connection on which this message was received
    /// </summary>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> that can be used to cancel publication in the event of shutdown</param>
    /// <returns>A task indicating the result of the publish</returns>
    /// <remarks>
    ///     Because this library uses the <see cref="ManagedMqttClient" />, the message will be queued for delivery locally. As such, the
    ///     <see cref="CancellationToken"/> will be ignored. The result does not necessarily indicate that the message was successfully
    ///     delivered to the broker, but rather that it has been queued for delivery.
    /// </remarks>
    Task PublishAsync(
        MqttApplicationMessage message,
        CancellationToken cancellationToken = default);
}
