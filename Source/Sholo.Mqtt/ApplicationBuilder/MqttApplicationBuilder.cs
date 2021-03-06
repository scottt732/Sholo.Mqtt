using System.Collections.Generic;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Sholo.Mqtt.Application;
using Sholo.Mqtt.Topics.FilterSanitizer;

namespace Sholo.Mqtt.ApplicationBuilder
{
    public class MqttApplicationBuilder : IMqttApplicationBuilder
    {
        private IDictionary<MqttTopicFilter, MqttRequestDelegate> TopicFilters { get; } = new Dictionary<MqttTopicFilter, MqttRequestDelegate>();

        public IMqttApplicationBuilder Map(
            string topicPattern,
            MqttRequestDelegate middleware,
            MqttQualityOfServiceLevel? qualityOfServiceLevel = null,
            bool? noLocal = null,
            bool? retainAsPublished = null,
            MqttRetainHandling? retainHandling = null)
        {
            var topicFilter = CreateTopicFilter(topicPattern, qualityOfServiceLevel, noLocal, retainAsPublished, retainHandling);

            TopicFilters.Add(
                topicFilter,
                async context =>
                {
                    if (IsMatch(topicFilter, context))
                    {
                        return await middleware.Invoke(context);
                    }

                    return false;
                }
            );

            return this;
        }

        public IMqttApplicationBuilder Use(MqttRequestDelegate middleware)
        {
            TopicFilters.Add(CreateTopicFilter("#"), middleware);
            return this;
        }

        public IMqttApplication Build()
            => new MqttApplication(
                TopicFilters.Keys,
                BuildRequestDelegate()
            );

        private static bool IsMatch(MqttTopicFilter topicFilter, IMqttRequestContext request)
            => MqttTopicFilterComparer.IsMatch(request.Topic, topicFilter.Topic)
               && request.QualityOfServiceLevel == topicFilter.QualityOfServiceLevel;

        private MqttTopicFilter CreateTopicFilter(
            string topic,
            MqttQualityOfServiceLevel? qualityOfServiceLevel = null,
            bool? noLocal = null,
            bool? retainAsPublished = null,
            MqttRetainHandling? retainHandling = null)
        {
            var result = new MqttTopicFilter
            {
                Topic = TopicFilterSanitizer.SanitizeTopic(topic)
            };

            if (qualityOfServiceLevel.HasValue)
            {
                result.QualityOfServiceLevel = qualityOfServiceLevel.Value;
            }

            if (noLocal.HasValue)
            {
                result.NoLocal = noLocal.Value;
            }

            if (retainAsPublished.HasValue)
            {
                result.RetainAsPublished = retainAsPublished.Value;
            }

            if (retainHandling.HasValue)
            {
                result.RetainHandling = retainHandling.Value;
            }

            return result;
        }

        private MqttRequestDelegate BuildRequestDelegate()
        {
            return async context =>
            {
                foreach (var component in TopicFilters)
                {
                    var topicFilter = component.Key;
                    var middleware = component.Value;

                    if (IsMatch(topicFilter, context))
                    {
                        if (await middleware.Invoke(context))
                        {
                            return true;
                        }
                    }
                }

                return false;
            };
        }
    }
}
