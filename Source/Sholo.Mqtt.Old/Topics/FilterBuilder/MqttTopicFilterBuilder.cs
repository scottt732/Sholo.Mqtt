using System;
using MQTTnet;
using MQTTnet.Protocol;
using Sholo.Mqtt.Old.Topics.Filter;
using Sholo.Mqtt.Old.Topics.FilterSanitizer;

namespace Sholo.Mqtt.Old.Topics.FilterBuilder
{
    internal class MqttTopicFilterBuilder : IMqttTopicFilterBuilder
    {
        private string Topic { get; set; }
        private MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }
        private bool? NoLocal { get; set; }
        private bool? RetainAsPublished { get; set; }
        private MqttRetainHandling? RetainHandling { get; set; }

        public IMqttTopicFilterBuilder WithQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel)
        {
            QualityOfServiceLevel = qualityOfServiceLevel;
            return this;
        }

        public IMqttTopicFilterBuilder WithTopic(string topicPattern)
        {
            Topic = topicPattern != null ? TopicFilterSanitizer.SanitizeTopic(topicPattern) : null;
            return this;
        }

        public IMqttTopicFilterBuilder WithNoLocal(bool noLocal)
        {
            NoLocal = noLocal;
            return this;
        }

        public IMqttTopicFilterBuilder WithRetainAsPublished(bool retainAsPublished)
        {
            RetainAsPublished = retainAsPublished;
            return this;
        }

        public IMqttTopicFilterBuilder WithRetainHandling(MqttRetainHandling retainHandling)
        {
            RetainHandling = retainHandling;
            return this;
        }

        public IMqttTopicFilter Build()
        {
            if (Topic == null)
            {
                throw new ArgumentNullException(nameof(Topic), $"The {nameof(Topic)} cannot be null");
            }

            if (Topic.Length == 0)
            {
                throw new ArgumentException($"The {nameof(Topic)} must be non-empty", nameof(Topic));
            }

            var result = new MqttTopicFilterImpl
            {
                Topic = Topic,
                QualityOfServiceLevel = QualityOfServiceLevel ?? MqttQualityOfServiceLevel.AtMostOnce
            };

            if (NoLocal.HasValue)
            {
                result.NoLocal = NoLocal.Value;
            }

            if (RetainAsPublished.HasValue)
            {
                result.RetainAsPublished = RetainAsPublished.Value;
            }

            if (RetainHandling.HasValue)
            {
                result.RetainHandling = RetainHandling.Value;
            }

            return result;
        }

        private class MqttTopicFilterImpl : MqttTopicFilter, IMqttTopicFilter
        {
        }
    }
}
