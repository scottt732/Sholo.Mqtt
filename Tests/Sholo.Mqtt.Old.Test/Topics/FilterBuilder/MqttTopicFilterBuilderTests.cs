using System;
using MQTTnet.Protocol;
using Sholo.Mqtt.Old.Topics.FilterBuilder;
using Xunit;

namespace Sholo.Mqtt.Old.Test.Topics.FilterBuilder
{
    public class MqttTopicFilterBuilderTests
    {
        private MqttTopicFilterBuilder MqttTopicFilterBuilder { get; }

        public MqttTopicFilterBuilderTests()
        {
            MqttTopicFilterBuilder = new MqttTopicFilterBuilder();
        }

        [Theory]
        [CombinatorialData]
        public void WithNoLocal_WhenBuilt_HasExpectedNoLocal(bool retainAsPublished)
        {
            MqttTopicFilterBuilder
                .WithTopic("#")
                .WithNoLocal(retainAsPublished);

            var mqttTopicFilter = MqttTopicFilterBuilder.Build();

            Assert.Equal(retainAsPublished, mqttTopicFilter.NoLocal);
        }

        [Theory]
        [CombinatorialData]
        public void WithRetainAsPublished_WhenBuilt_HasExpectedRetainAsPublished(bool retainAsPublished)
        {
            MqttTopicFilterBuilder
                .WithTopic("#")
                .WithRetainAsPublished(retainAsPublished);

            var mqttTopicFilter = MqttTopicFilterBuilder.Build();

            Assert.Equal(retainAsPublished, mqttTopicFilter.RetainAsPublished);
        }

        [Theory]
        [CombinatorialData]
        public void WithRetainHandling_WhenBuilt_HasExpectedRetainHandling(MqttRetainHandling retainHandling)
        {
            MqttTopicFilterBuilder
                .WithTopic("#")
                .WithRetainHandling(retainHandling);

            var mqttTopicFilter = MqttTopicFilterBuilder.Build();

            Assert.Equal(retainHandling, mqttTopicFilter.RetainHandling);
        }

        [Theory]
        [CombinatorialData]
        public void WithQualityOfServiceLevel_WhenBuilt_HasExpectedQualityOfServiceLevel(MqttQualityOfServiceLevel qualityOfServiceLevel)
        {
            MqttTopicFilterBuilder
                .WithTopic("#")
                .WithQualityOfServiceLevel(qualityOfServiceLevel);

            var mqttTopicFilter = MqttTopicFilterBuilder.Build();

            Assert.Equal(qualityOfServiceLevel, mqttTopicFilter.QualityOfServiceLevel);
        }

        [Fact]
        public void WithoutParameters_WhenBuilt_HasExpectedDefaultValues()
        {
            MqttTopicFilterBuilder
                .WithTopic("#");

            var mqttTopicFilter = MqttTopicFilterBuilder.Build();

            Assert.Null(mqttTopicFilter.RetainHandling);
            Assert.Null(mqttTopicFilter.NoLocal);
            Assert.Null(mqttTopicFilter.RetainAsPublished);
            Assert.Equal(MqttQualityOfServiceLevel.AtMostOnce, mqttTopicFilter.QualityOfServiceLevel);
        }

        [Theory]
        [InlineData("#")]
        public void WithTopic_WhenBuilt_HasExpectedTopic(string topic)
        {
            MqttTopicFilterBuilder.WithTopic(topic);

            var mqttTopicFilter = MqttTopicFilterBuilder.Build();

            Assert.Equal(topic, mqttTopicFilter.Topic);
        }

        [Fact]
        public void WithoutTopic_WhenBuilt_ThrowsArgumentNullException()
        {
            MqttTopicFilterBuilder.WithTopic(null);

            Assert.Throws<ArgumentNullException>(() => MqttTopicFilterBuilder.Build());
        }

        [Fact]
        public void WithoutTopic_WhenBuilt_ThrowsArgumentException()
        {
            MqttTopicFilterBuilder.WithTopic(string.Empty);

            Assert.Throws<ArgumentException>(() => MqttTopicFilterBuilder.Build());
        }
    }
}
