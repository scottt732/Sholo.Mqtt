using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using MQTTnet;
using MQTTnet.Client.Publishing;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using Sholo.Mqtt.ApplicationProvider;
using Sholo.Mqtt.Consumer;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.Test.Helpers;
using Xunit;

namespace Sholo.Mqtt.Test.Consumer
{
    public class MqttConsumerServiceTests
    {
        private Mock<IManagedMqttClient> MockManagedMqttClient { get; } = new Mock<IManagedMqttClient>();
        private Mock<IMqttApplicationProvider> MockMqttApplicationProvider { get; } = new Mock<IMqttApplicationProvider>();
        private Mock<IServiceScopeFactory> MockServiceScopeFactory { get; } = new Mock<IServiceScopeFactory>();
        private Mock<IManagedMqttClientOptions> MockManagedMqttClientOptions { get; } = new Mock<IManagedMqttClientOptions>();
        private TestLogger<MqttConsumerService<ManagedMqttSettings>> Logger { get; } = new TestLogger<MqttConsumerService<ManagedMqttSettings>>();

        [Fact]
        public async Task ExecuteAsync_WithUnconfiguredApplication_BehavesAsExpected()
        {
            using var mqttConsumerService = CreateMqttConsumerService(s =>
            {
                s.Host = "mqtt.local";
                s.Port = 1884;
            });

            var testApplication = MqttApplicationHelper.CreateDoesNotHandleRequests();

            Assert.Empty(testApplication.TopicFilters);

            MockMqttApplicationProvider
                .SetupGet(x => x.Current)
                .Returns(testApplication);

            MockManagedMqttClient
                .Setup(c => c.StartAsync(MockManagedMqttClientOptions.Object))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await mqttConsumerService.StartAsync(CancellationToken.None);

            Assert.Collection(
                Logger.LogEntries,
                l => { Assert.Equal("Connecting to MQTT broker at mqtt.local:1884...", l.Message); });

            MockManagedMqttClient.VerifyAll();

            MockManagedMqttClient
                .Setup(c => c.StopAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            await mqttConsumerService.StopAsync(CancellationToken.None);

            MockMqttApplicationProvider.VerifyAll();
            MockManagedMqttClient.VerifyAll();

            Assert.Collection(
                Logger.LogEntries,
                l => { Assert.Equal("Connecting to MQTT broker at mqtt.local:1884...", l.Message); },
                l => { Assert.Equal("Shutting down MQTT broker connection...", l.Message); });
        }

        [Fact]
        public async Task ExecuteAsync_WithDefaultApplication_BehavesAsExpected()
        {
            using var mqttConsumerService = CreateMqttConsumerService(s =>
            {
                s.Host = "mqtt.local";
                s.Port = 1884;
            });

            var testApplication = MqttApplicationHelper.CreateHandlesAllRequests();

            Assert.Collection(
                testApplication.TopicFilters,
                i => Assert.Equal("#", i.Topic));

            MockMqttApplicationProvider
                .SetupGet(x => x.Current)
                .Returns(testApplication);

            MockManagedMqttClient
                .Setup(c => c.StartAsync(MockManagedMqttClientOptions.Object))
                .Returns(Task.CompletedTask)
                .Verifiable();

            MockManagedMqttClient
                .Setup(c => c.SubscribeAsync(testApplication.TopicFilters))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await mqttConsumerService.StartAsync(CancellationToken.None);

            Assert.Collection(
                Logger.LogEntries,
                l => { Assert.Equal("Connecting to MQTT broker at mqtt.local:1884...", l.Message); });

            MockManagedMqttClient.VerifyAll();

            MockManagedMqttClient
                .Setup(c => c.StopAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            await mqttConsumerService.StopAsync(CancellationToken.None);

            MockMqttApplicationProvider.VerifyAll();
            MockManagedMqttClient.VerifyAll();

            Assert.Collection(
                Logger.LogEntries,
                l => { Assert.Equal("Connecting to MQTT broker at mqtt.local:1884...", l.Message); },
                l => { Assert.Equal("Shutting down MQTT broker connection...", l.Message); });
        }

        [Fact]
        public async Task ExecuteAsync_WithOnlineMessageAndDefaultApplication_BehavesAsExpected()
        {
            using var mqttConsumerService = CreateMqttConsumerService(s =>
            {
                s.Host = "mqtt.local";
                s.Port = 1884;
                s.OnlineMessage = new MqttMessageSettings
                {
                    Topic = "test/availability",
                    Payload = "online",
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
                    Retain = true
                };
            });

            var testApplication = MqttApplicationHelper.CreateHandlesAllRequests();

            Assert.Collection(
                testApplication.TopicFilters,
                i => Assert.Equal("#", i.Topic));

            MockMqttApplicationProvider
                .SetupGet(x => x.Current)
                .Returns(testApplication);

            MockManagedMqttClient
                .Setup(c => c.StartAsync(MockManagedMqttClientOptions.Object))
                .Returns(Task.CompletedTask)
                .Verifiable();

            MockManagedMqttClient
                .Setup(c => c.SubscribeAsync(testApplication.TopicFilters))
                .Returns(Task.CompletedTask)
                .Verifiable();

            MockManagedMqttClient
                .Setup(
                    c => c.PublishAsync(
                        It.Is<MqttApplicationMessage>(
                            m =>
                                m.Topic.Equals("test/availability") &&
                                m.Payload.SequenceEqual(Encoding.UTF8.GetBytes("online")) &&
                                m.QualityOfServiceLevel == MqttQualityOfServiceLevel.ExactlyOnce &&
                                m.Retain),
                        It.Is<CancellationToken>(ct => !ct.IsCancellationRequested)
                    ))
                .ReturnsAsync(new MqttClientPublishResult())
                .Verifiable();

            await mqttConsumerService.StartAsync(CancellationToken.None);

            Assert.Collection(
                Logger.LogEntries,
                l => { Assert.Equal("Connecting to MQTT broker at mqtt.local:1884...", l.Message); },
                l => { Assert.Equal("Sending online message to test/availability", l.Message); });

            MockManagedMqttClient.VerifyAll();

            MockManagedMqttClient
                .Setup(c => c.StopAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            await mqttConsumerService.StopAsync(CancellationToken.None);

            MockMqttApplicationProvider.VerifyAll();
            MockManagedMqttClient.VerifyAll();

            Assert.Collection(
                Logger.LogEntries,
                l => { Assert.Equal("Connecting to MQTT broker at mqtt.local:1884...", l.Message); },
                l => { Assert.Equal("Sending online message to test/availability", l.Message); },
                l => { Assert.Equal("Shutting down MQTT broker connection...", l.Message); });
        }

        [Fact]
        public async Task ExecuteAsync_WhenApplicationChanges_UpdatesSubscriptions()
        {
            var mqttApplicationProvider = MqttApplicationProviderHelper.CreateMqttApplicationProvider(2);

            mqttApplicationProvider.Rebuild();

            using var mqttConsumerService = CreateMqttConsumerService(
                s =>
                {
                    s.Host = "mqtt.local";
                    s.Port = 1884;
                },
                mqttApplicationProviderOverride: mqttApplicationProvider);

            Assert.Collection(
                mqttApplicationProvider.Current.TopicFilters,
                i => Assert.Equal("test/builder_1/build_1", i.Topic),
                i => Assert.Equal("test/builder_2/build_1", i.Topic));

            MockManagedMqttClient
                .Setup(c => c.StartAsync(MockManagedMqttClientOptions.Object))
                .Returns(Task.CompletedTask)
                .Verifiable();

            MockManagedMqttClient
                .Setup(c => c.SubscribeAsync(mqttApplicationProvider.Current.TopicFilters))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await mqttConsumerService.StartAsync(CancellationToken.None);

            MockManagedMqttClient
                .Setup(c => c.SubscribeAsync(It.Is<MqttTopicFilter[]>(
                    f => f.Length == 2 &&
                        f[0].Topic.Equals("test/builder_1/build_1") &&
                        f[1].Topic.Equals("test/builder_2/build_1"))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            MockManagedMqttClient
                .Setup(c => c.SubscribeAsync(It.Is<MqttTopicFilter[]>(
                    f => f.Length == 2 &&
                         f[0].Topic.Equals("test/builder_1/build_2") &&
                         f[1].Topic.Equals("test/builder_2/build_2"))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var raisedEvent = Assert.Raises<ApplicationChangedEventArgs>(
                x => mqttApplicationProvider.ApplicationChanged += x,
                x => mqttApplicationProvider.ApplicationChanged -= x,
                () => mqttApplicationProvider.Rebuild());

            Assert.Collection(
                raisedEvent.Arguments.Previous.TopicFilters,
                f => Assert.Equal("test/builder_1/build_1", f.Topic),
                f => Assert.Equal("test/builder_2/build_1", f.Topic));

            Assert.Collection(
                raisedEvent.Arguments.Current.TopicFilters,
                f => Assert.Equal("test/builder_1/build_2", f.Topic),
                f => Assert.Equal("test/builder_2/build_2", f.Topic));

            MockManagedMqttClient.VerifyAll();
        }

        private MqttConsumerService<ManagedMqttSettings> CreateMqttConsumerService(
            Action<ManagedMqttSettings> managedMqttSettingsConfiguration,
            IManagedMqttClient managedMqttClientOverride = null,
            IMqttApplicationProvider mqttApplicationProviderOverride = null,
            IServiceScopeFactory serviceScopeFactoryOverride = null,
            IManagedMqttClientOptions managedMqttClientOptionsOverride = null
            )
        {
            var managedMqttSettings = new ManagedMqttSettings();
            managedMqttSettingsConfiguration.Invoke(managedMqttSettings);

            var managedMqttSettingsOptionsWrapper = new OptionsWrapper<ManagedMqttSettings>(managedMqttSettings);

            return new MqttConsumerService<ManagedMqttSettings>(
                managedMqttClientOverride ?? MockManagedMqttClient.Object,
                mqttApplicationProviderOverride ?? MockMqttApplicationProvider.Object,
                serviceScopeFactoryOverride ?? MockServiceScopeFactory.Object,
                managedMqttClientOptionsOverride ?? MockManagedMqttClientOptions.Object,
                managedMqttSettingsOptionsWrapper,
                Logger
            );
        }
    }
}
