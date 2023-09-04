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
using Sholo.Mqtt.Application.Provider;
using Sholo.Mqtt.Settings;

namespace Sholo.Mqtt.Consumer
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
            var blockUntilStopRequested = new SemaphoreSlim(0);

            // ReSharper disable once AccessToDisposedClosure
            stoppingToken.Register(() => blockUntilStopRequested.Release(1));

            StoppingToken = stoppingToken;
            Logger.LogInformation("Connecting to MQTT broker at {Host}:{Port}...", MqttSettings.Value.Host, MqttSettings.Value.Port ?? 1883);

            await MqttClient.StartAsync(Options);

            ApplicationProvider.Rebuild();

            var onlineMessage = MqttSettings.Value.GetOnlineApplicationMessage();
            if (onlineMessage != null)
            {
                Logger.LogInformation("Sending online message to {Topic}", onlineMessage.Topic);
                await MqttClient.PublishAsync(onlineMessage, stoppingToken);
            }

            await blockUntilStopRequested.WaitAsync(CancellationToken.None);

            Logger.LogInformation("Shutting down MQTT broker connection...");
            await MqttClient.StopAsync();

            blockUntilStopRequested.Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler")]
        private async void OnApplicationChanged(object sender, ApplicationChangedEventArgs e)
        {
            var previousTopicFilters = e.Previous?.TopicFilters.ToArray() ?? Array.Empty<MqttTopicFilter>();
            var currentTopicFilters = e.Current?.TopicFilters.ToArray() ?? Array.Empty<MqttTopicFilter>();

            if (previousTopicFilters.Any() || currentTopicFilters.Any())
            {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (e.Previous == null)
                {
                    Logger.LogInformation("MQTT topic subscriptions:");
                }
                else
                {
                    Logger.LogInformation("Updating MQTT topic subscriptions");
                }

                await UnsubscribeTopics(previousTopicFilters);
                await SubscribeToTopics(currentTopicFilters);
            }
        }

        private void OnSynchronizingSubscriptionsFailed(ManagedProcessFailedEventArgs eventArgs)
        {
            Logger.LogError(eventArgs.Exception, "Failed to synchronize subscriptions: {Message}", eventArgs.Exception.Message);
        }

        private void OnConnectingFailed(ManagedProcessFailedEventArgs eventArgs)
        {
            if (eventArgs.Exception != null)
            {
                Logger.LogError(eventArgs.Exception, "Failed to connect: {Message}", eventArgs.Exception.Message);
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
                Logger.LogError(eventArgs.Exception, "Attempting to restore MQTT broker connection: {Message}", eventArgs.Exception.Message);
            }
            else if (eventArgs.ClientWasConnected)
            {
                Logger.LogWarning("Attempting to restore MQTT broker connection...");
            }
            else
            {
                Logger.LogWarning("Attempting to restore MQTT broker connection...");
            }

            LogUnsuccessfulAuthenticationResult(eventArgs.Reason);
        }

        private void OnConnected(MqttClientConnectedEventArgs eventArgs)
        {
            Logger.LogInformation("Connected to MQTT broker:");
            Logger.LogInformation("  Result .............................. {ResultCode}", eventArgs.ConnectResult.ResultCode);
            Logger.LogInformation("  Session Present ..................... {IsSessionPresent}", eventArgs.ConnectResult.IsSessionPresent ? "Yes" : "No");
            Logger.LogInformation("  Wildcard Subscription Available ..... {WildcardSubscriptionAvailable}", eventArgs.ConnectResult.WildcardSubscriptionAvailable ? "Yes" : "No");
            Logger.LogInformation("  Retain Available .................... {RetainAvailable}", eventArgs.ConnectResult.RetainAvailable ? "Yes" : "No");
            Logger.LogInformation("  Assigned Client Identifier .......... {AssignedClientIdentifier}", eventArgs.ConnectResult.AssignedClientIdentifier);
            Logger.LogInformation("  Authentication Method ............... {AuthenticationMethod}", eventArgs.ConnectResult.AuthenticationMethod);
            Logger.LogInformation("  Maximum Packet Size ................. {MaximumPacketSize}", eventArgs.ConnectResult.MaximumPacketSize);
            Logger.LogInformation("  Reason .............................. {Reason}", eventArgs.ConnectResult.ReasonString);
            Logger.LogInformation("  Receive Maximum ..................... {ReceiveMaximum}", eventArgs.ConnectResult.ReceiveMaximum);
            Logger.LogInformation("  Maximum QoS ......................... {MaximumQoS}", eventArgs.ConnectResult.MaximumQoS);
            Logger.LogInformation("  Response Information ................ {ReceiveMaximum}", eventArgs.ConnectResult.ResponseInformation);
            Logger.LogInformation("  Topic Alias Maximum ................. {TopicAliasMaximum}", eventArgs.ConnectResult.TopicAliasMaximum == 0 ? "Not supported" : eventArgs.ConnectResult.TopicAliasMaximum);
            Logger.LogInformation("  Server Reference .................... {ServerReference}", eventArgs.ConnectResult.ServerReference);
            Logger.LogInformation("  Server Keep Alive ................... {ServerKeepAlive}", eventArgs.ConnectResult.ServerKeepAlive != null ? eventArgs.ConnectResult.ServerKeepAlive.Value.ToString() : "N/A");
            Logger.LogInformation("  Session Expiry Interval ............. {SessionExpiryInterval}", eventArgs.ConnectResult.SessionExpiryInterval != null ? eventArgs.ConnectResult.SessionExpiryInterval.Value.ToString() : "N/A");
            Logger.LogInformation("  Subscription Identifiers Available .. {SubscriptionIdentifiersAvailable}", eventArgs.ConnectResult.SubscriptionIdentifiersAvailable ? "Yes" : "No");
            Logger.LogInformation("  Shared Subscription Available ....... {SharedSubscriptionAvailable}", eventArgs.ConnectResult.SharedSubscriptionAvailable ? "Yes" : "No");
            Logger.LogInformation("  User Properties ..................... {HasUserProperties}", eventArgs.ConnectResult.UserProperties?.Count > 0 ? string.Empty : "N/A");

            if (eventArgs.ConnectResult.UserProperties?.Count > 0)
            {
                foreach (var userProperty in eventArgs.ConnectResult.UserProperties)
                {
                    Logger.LogInformation("    {Name}: {Value}", userProperty.Name, userProperty.Value);
                }
            }
        }

        private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var application = ApplicationProvider.Current;

            if (application?.RequestDelegate == null)
            {
                eventArgs.ProcessingFailed = true;
                return;
            }

            using var scope = ServiceScopeFactory.CreateScope();

            var context = new MqttRequestContext(scope.ServiceProvider, eventArgs.ApplicationMessage, eventArgs.ClientId);

            var success = false;

            try
            {
                success = await application.RequestDelegate?.Invoke(context);

                if (!success)
                {
                    Logger.LogWarning("No handler found for message on {Topic}", context.Topic);
                }
            }
            catch (Exception exc)
            {
                Logger.LogError(exc, "Request failed to process: {Message}", exc.Message);
            }

            eventArgs.ProcessingFailed = !success;
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

            if (currentTopicFilters.Length > 0)
            {
                foreach (var topicFilter in currentTopicFilters)
                {
                    Logger.LogInformation(
                        " - {Topic} | QoS={QoS} NoLocal={NoLocal} RetainAsPublished={RetainAsPublished} RetainHandling={RetainHandling}",
                        topicFilter.Topic,
                        topicFilter.QualityOfServiceLevel,
                        topicFilter.NoLocal,
                        topicFilter.RetainAsPublished,
                        topicFilter.RetainHandling);
                }

                await MqttClient.SubscribeAsync(currentTopicFilters);
            }
        }

        private void LogUnsuccessfulAuthenticationResult(MqttClientDisconnectReason reason)
        {
            if (!IsShuttingDown && reason != MqttClientDisconnectReason.NormalDisconnection)
            {
                Logger.LogWarning("Authentication Result: {Reason}", reason);
            }
        }
    }
}
