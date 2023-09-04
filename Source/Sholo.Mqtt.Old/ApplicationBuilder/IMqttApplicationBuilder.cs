using System;
using JetBrains.Annotations;
using Sholo.Mqtt.Old.Application;
using Sholo.Mqtt.Old.Topics.Filter;
using Sholo.Mqtt.Old.Topics.FilterBuilder;

namespace Sholo.Mqtt.Old.ApplicationBuilder
{
    [PublicAPI]
    public interface IMqttApplicationBuilder
    {
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
        */

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
    }
}
