using Sholo.Mqtt.ApplicationProvider;
using Sholo.Mqtt.Test.Helpers;
using Xunit;

namespace Sholo.Mqtt.Test.ApplicationProvider
{
    public class MqttApplicationProviderTests
    {
        [Fact]
        public void Rebuild_WhenInvoked_RaisesApplicationChangedEvent()
        {
            var mqttApplicationProvider = MqttApplicationProviderHelper.CreateMqttApplicationProvider(2);

            Assert.Null(mqttApplicationProvider.Current);

            var raisedEvent1 = Assert.Raises<ApplicationChangedEventArgs>(
                x => mqttApplicationProvider.ApplicationChanged += x,
                x => mqttApplicationProvider.ApplicationChanged -= x,
                () => mqttApplicationProvider.Rebuild());

            Assert.Null(raisedEvent1.Arguments.Previous);
            Assert.NotNull(raisedEvent1.Arguments.Current);

            Assert.NotNull(mqttApplicationProvider.Current);

            Assert.Collection(
                raisedEvent1.Arguments.Current.TopicFilters,
                topicFilter1 => Assert.Equal("test/builder_1/build_1", topicFilter1.Topic),
                topicFilter2 => Assert.Equal("test/builder_2/build_1", topicFilter2.Topic));

            var raisedEvent2 = Assert.Raises<ApplicationChangedEventArgs>(
                x => mqttApplicationProvider.ApplicationChanged += x,
                x => mqttApplicationProvider.ApplicationChanged -= x,
                () => mqttApplicationProvider.Rebuild());

            Assert.NotNull(raisedEvent2.Arguments.Previous);
            Assert.NotNull(raisedEvent2.Arguments.Current);

            Assert.Same(raisedEvent1.Arguments.Current, raisedEvent2.Arguments.Previous);
            Assert.NotSame(raisedEvent2.Arguments.Previous, raisedEvent2.Arguments.Current);

            Assert.Collection(
                raisedEvent2.Arguments.Current.TopicFilters,
                topicFilter1 => Assert.Equal("test/builder_1/build_2", topicFilter1.Topic),
                topicFilter2 => Assert.Equal("test/builder_2/build_2", topicFilter2.Topic));
        }
    }
}
