using System;
using System.Collections.Generic;

namespace Sholo.Mqtt.Application.Builder;

[PublicAPI]
public interface IMqttApplicationBuilder
{
    /// <summary>
    /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the application's service container.
    /// </summary>
    IServiceProvider ApplicationServices { get; set; }

    /*
    /// <summary>
    /// Gets the set of HTTP features the application's server provides.
    /// </summary>
    IFeatureCollection ServerFeatures { get; }
    */

    /// <summary>
    /// Gets a key/value collection that can be used to share data between middleware.
    /// </summary>
    IDictionary<string, object> Properties { get; }

    /// <summary>
    /// Adds a middleware delegate to the application's request pipeline.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The <see cref="IMqttApplicationBuilder"/>.</returns>
    IMqttApplicationBuilder Use(Func<MqttRequestDelegate, MqttRequestDelegate> middleware);

    /// <summary>
    /// Creates a new <see cref="IMqttApplicationBuilder"/> that shares the <see cref="Properties"/> of this
    /// <see cref="IMqttApplicationBuilder"/>.
    /// </summary>
    /// <returns>The new <see cref="IMqttApplicationBuilder"/>.</returns>
#pragma warning disable CA1716
    IMqttApplicationBuilder New();
#pragma warning restore CA1716

    /// <summary>
    /// Builds the delegate used by this application to process MQTT requests.
    /// </summary>
    /// <returns>The request handling delegate.</returns>
    IMqttApplication Build();

    /*
    string PathPrefix { get; }

    // 1a
    IMqttApplicationBuilder Map(
        IMqttTopicFilter topicFilter,
        MqttRequestDelegate requestDelegate);

    IMqttApplicationBuilder Map(
        Action<IMqttTopicFilterBuilder> topicFilterConfiguration,
        MqttRequestDelegate requestDelegate);

    /*
    IMqttApplicationBuilder Map(
        IMqttTopicFilter topicFilter,
        Action<MqttRequestDelegate requestDelegate);
    * /

    IMqttApplicationBuilder Map<TPayload>(
        IMqttTopicFilter topicFilter,
        TypedMqttRequestDelegate<TPayload> requestDelegate)
            where TPayload : class;

    IMqttApplicationBuilder Map<TTopicParameters>(
        IMqttTopicFilter topicFilter,
        MqttRequestDelegate<TTopicParameters> requestDelegate)
            where TTopicParameters : class;

    IMqttApplicationBuilder Map<TTopicParameters, TPayload>(
        IMqttTopicFilter topicFilter,
        TypedMqttRequestDelegate<TTopicParameters, TPayload> requestDelegate)
            where TTopicParameters : class
            where TPayload : class;

    IMqttApplicationBuilder Use(MqttRequestDelegate requestDelegate);

    IMqttApplicationBuilder Use<TPayload>(
        TypedMqttRequestDelegate<TPayload> requestDelegate)
            where TPayload : class;

    IMqttApplication Build();
    */
}
