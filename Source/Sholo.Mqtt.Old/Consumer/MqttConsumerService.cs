using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Sholo.Mqtt.Old.ApplicationProvider;
using Sholo.Mqtt.Old.Settings;

namespace Sholo.Mqtt.Old.Consumer
{
    public sealed class MqttConsumerService<TManagedMqttSettings> : BackgroundService
        where TManagedMqttSettings : ManagedMqttSettings, new()
    {
        private IManagedMqttClient MqttClient { get; }
        private IMqttApplicationProvider ApplicationProvider { get; }
        private IServiceScopeFactory ServiceScopeFactory { get; }
        private IManagedMqttClientOptions Options { get; }
        private IOptions<TManagedMqttSettings> MqttSettings { get; }
        private ILogger Logger { get; }

        private bool IsShuttingDown => StoppingToken?.IsCancellationRequested ?? false;
        private CancellationToken? StoppingToken { get; set; }

        public MqttConsumerService(
            IManagedMqttClient mqttClient,
            IMqttApplicationProvider applicationProvider,
            IServiceScopeFactory serviceScopeFactory,
            IManagedMqttClientOptions mqttClientOptions,
            IOptions<TManagedMqttSettings> mqttSettings,
            ILogger<MqttConsumerService<TManagedMqttSettings>> logger)
        {
            MqttClient = mqttClient;
            ApplicationProvider = applicationProvider;
            ServiceScopeFactory = serviceScopeFactory;
            Options = mqttClientOptions;
            MqttSettings = mqttSettings;
            Logger = logger;

            MqttClient.ApplicationMessageProcessedHandler = new ApplicationMessageProcessedHandlerDelegate(OnApplicationMessageProcessed);
            MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnApplicationMessageReceived);
            MqttClient.ApplicationMessageSkippedHandler = new ApplicationMessageSkippedHandlerDelegate(OnApplicationMessageSkipped);

            MqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            MqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            MqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            MqttClient.SynchronizingSubscriptionsFailedHandler = new SynchronizingSubscriptionsFailedHandlerDelegate(OnSynchronizingSubscriptionsFailed);

            ApplicationProvider.ApplicationChanged += OnApplicationChanged;
        }

        public override void Dispose()
        {
            ApplicationProvider.ApplicationChanged -= OnApplicationChanged;
            MqttClient?.Dispose();
            base.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var blockUntilStopRequested = new SemaphoreSlim(0);

            // ReSharper disable once AccessToDisposedClosure
            stoppingToken.Register(() => blockUntilStopRequested.Release(1));

            StoppingToken = stoppingToken;
            Logger.LogInformation($"Connecting to MQTT broker at {MqttSettings.Value.Host}:{MqttSettings.Value.Port ?? 1883}...");

            await MqttClient.StartAsync(Options);

            await SubscribeToTopics(ApplicationProvider.Current?.TopicFilters);

            var onlineMessage = MqttSettings.Value.GetOnlineApplicationMessage();
            if (onlineMessage != null)
            {
                Logger.LogInformation($"Sending online message to {onlineMessage.Topic}");
                await MqttClient.PublishAsync(onlineMessage, stoppingToken);
            }

            await blockUntilStopRequested.WaitAsync(CancellationToken.None);

            Logger.LogInformation("Shutting down MQTT broker connection...");
            await MqttClient.StopAsync();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler")]
        private async void OnApplicationChanged(object sender, ApplicationChangedEventArgs e)
        {
            Logger.LogWarning($"Application change detected.");
            var previousTopicFilters = e.Previous?.TopicFilters;
            var currentTopicFilters = e.Current?.TopicFilters;

            await UnsubscribeTopics(previousTopicFilters);
            await SubscribeToTopics(currentTopicFilters);
        }

        private void OnSynchronizingSubscriptionsFailed(ManagedProcessFailedEventArgs eventArgs)
        {
            Logger.LogError(eventArgs.Exception, $"Failed to synchronize subscriptions: {eventArgs.Exception.Message}");
        }

        private void OnConnectingFailed(ManagedProcessFailedEventArgs eventArgs)
        {
            if (eventArgs.Exception != null)
            {
                Logger.LogError(eventArgs.Exception, $"Failed to connect: {eventArgs.Exception.Message}");
            }
            else
            {
                Logger.LogError("Failed to connect");
            }
        }

        private void OnDisconnected(MqttClientDisconnectedEventArgs eventArgs)
        {
            if (IsShuttingDown)
            {
                Logger.LogDebug("Disconnected due to shutdown request.");
            }
            else if (eventArgs.Exception != null)
            {
                Logger.LogError(eventArgs.Exception, $"Attempting to restore MQTT broker connection: {eventArgs.Exception.Message}");
            }
            else if (eventArgs.ClientWasConnected)
            {
                Logger.LogWarning("Attempting to restore MQTT broker connection...");
            }
            else
            {
                Logger.LogWarning("Attempting to restore MQTT broker connection...");
            }

            LogUnsuccessfulAuthenticationResult(eventArgs.AuthenticateResult);
        }

        private void OnConnected(MqttClientConnectedEventArgs eventArgs)
        {
            Logger.LogInformation("Connected to MQTT broker.");

            LogUnsuccessfulAuthenticationResult(eventArgs.AuthenticateResult);
        }

        private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var application = ApplicationProvider.Current;

            if (application?.RequestDelegate == null)
            {
                eventArgs.ProcessingFailed = true;
                return;
            }

            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var success = false;
                var context = new MqttRequestContext(scope.ServiceProvider, eventArgs.ApplicationMessage, eventArgs.ClientId);

                try
                {
                    success = await application.RequestDelegate?.Invoke(context);
                }
                catch (Exception exc)
                {
                    Logger.LogError(exc, $"Request failed to process: {exc.Message}");
                }

                eventArgs.ProcessingFailed = !success;
            }
        }

        private void OnApplicationMessageProcessed(ApplicationMessageProcessedEventArgs eventArgs)
        {
            Logger.LogDebug("Request processed.");
        }

        private void OnApplicationMessageSkipped(ApplicationMessageSkippedEventArgs eventArgs)
        {
            Logger.LogWarning("Request skipped.");
        }

        private async Task UnsubscribeTopics(MqttTopicFilter[] previousTopicFilters)
        {
            if (previousTopicFilters == null)
            {
                return;
            }

            var topics = previousTopicFilters.Select(x => x.Topic).ToArray();
            await MqttClient.UnsubscribeAsync(topics);
        }

        private async Task SubscribeToTopics(MqttTopicFilter[] currentTopicFilters)
        {
            if (currentTopicFilters == null)
            {
                return;
            }

            await MqttClient.SubscribeAsync(currentTopicFilters);
        }

        private void LogUnsuccessfulAuthenticationResult(MqttClientAuthenticateResult authenticationResult)
        {
            if (!IsShuttingDown && authenticationResult != null && authenticationResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                Logger.LogWarning(
                    "Authentication Result: " +
                    authenticationResult.ResultCode +
                    (
                        !string.IsNullOrEmpty(authenticationResult.ReasonString)
                            ? $": {authenticationResult.ReasonString}"
                            : string.Empty)
                );
            }
        }
    }
}
