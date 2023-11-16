using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Sholo.Mqtt.Application.Provider;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.Consumer;

public sealed class MqttConsumerService<TManagedMqttSettings> : BackgroundService
    where TManagedMqttSettings : ManagedMqttSettings, new()
{
    private IManagedMqttClient MqttClient { get; }
    private IMqttApplicationProvider ApplicationProvider { get; }
    private IServiceScopeFactory ServiceScopeFactory { get; }
    private ManagedMqttClientOptions Options { get; }
    private IOptions<TManagedMqttSettings> MqttSettings { get; }
    private ILogger Logger { get; }

    private bool Initialized { get; set; }
    private bool IsShuttingDown => StoppingToken?.IsCancellationRequested ?? false;
    private CancellationToken? StoppingToken { get; set; }

    public MqttConsumerService(
        IManagedMqttClient mqttClient,
        IMqttApplicationProvider applicationProvider,
        IServiceScopeFactory serviceScopeFactory,
        ManagedMqttClientOptions mqttClientOptions,
        IOptions<TManagedMqttSettings> mqttSettings,
        ILogger<MqttConsumerService<TManagedMqttSettings>> logger)
    {
        MqttClient = mqttClient;
        ApplicationProvider = applicationProvider;
        ServiceScopeFactory = serviceScopeFactory;
        Options = mqttClientOptions;
        MqttSettings = mqttSettings;
        Logger = logger;
    }

    public override void Dispose()
    {
        if (Initialized)
        {
            MqttClient.ApplicationMessageProcessedAsync -= OnApplicationMessageProcessedAsync;
            MqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;
            MqttClient.ApplicationMessageSkippedAsync -= OnApplicationMessageSkippedAsync;
            MqttClient.ConnectedAsync -= OnConnectedAsync;
            MqttClient.DisconnectedAsync -= OnDisconnectedAsync;
            MqttClient.ConnectingFailedAsync -= OnConnectingFailedAsync;
            MqttClient.SynchronizingSubscriptionsFailedAsync -= OnSynchronizingSubscriptionsFailedAsync;
            ApplicationProvider.ApplicationChanged -= OnApplicationChanged;
        }

        MqttClient.Dispose();
        base.Dispose();
    }

    [ExcludeFromCodeCoverage]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqttClient.ApplicationMessageProcessedAsync += OnApplicationMessageProcessedAsync;
        MqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
        MqttClient.ApplicationMessageSkippedAsync += OnApplicationMessageSkippedAsync;
        MqttClient.ConnectedAsync += OnConnectedAsync;
        MqttClient.DisconnectedAsync += OnDisconnectedAsync;
        MqttClient.ConnectingFailedAsync += OnConnectingFailedAsync;
        MqttClient.SynchronizingSubscriptionsFailedAsync += OnSynchronizingSubscriptionsFailedAsync;
        ApplicationProvider.ApplicationChanged += OnApplicationChanged;
        Initialized = true;

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
            await MqttClient.EnqueueAsync(onlineMessage);
        }

        await blockUntilStopRequested.WaitAsync(CancellationToken.None);

        Logger.LogInformation("Shutting down MQTT broker connection...");
        await MqttClient.StopAsync();

        blockUntilStopRequested.Dispose();
    }

    [ExcludeFromCodeCoverage]
    [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler")]
    private async void OnApplicationChanged(object? sender, ApplicationChangedEventArgs e)
    {
        var previousTopicFilters = e.Previous?.TopicFilters.ToArray() ?? Array.Empty<IMqttTopicFilter>();
        var currentTopicFilters = e.Current.TopicFilters.ToArray();

        if (previousTopicFilters.Length > 0 || currentTopicFilters.Length > 0)
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

            await UnsubscribeTopicsAsync(previousTopicFilters);
            await SubscribeToTopicsAsync(currentTopicFilters);
        }
    }

    [ExcludeFromCodeCoverage]
    private Task OnSynchronizingSubscriptionsFailedAsync(ManagedProcessFailedEventArgs eventArgs)
    {
        Logger.LogError(eventArgs.Exception, "Failed to synchronize subscriptions: {Message}", eventArgs.Exception.Message);
        return Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private Task OnConnectingFailedAsync(ConnectingFailedEventArgs eventArgs)
    {
        if (eventArgs.Exception != null)
        {
            Logger.LogError(eventArgs.Exception, "Failed to connect: {Message}", eventArgs.Exception.Message);
        }
        else
        {
            Logger.LogError("Failed to connect");
        }

        return Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
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

        LogDisconnectEvent(eventArgs);
        return Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private Task OnConnectedAsync(MqttClientConnectedEventArgs eventArgs)
    {
        Logger.LogInformation("Connected to MQTT broker");
        Logger.LogDebug("  Result .............................. {ResultCode}", eventArgs.ConnectResult.ResultCode);
        Logger.LogDebug("  Session Present ..................... {IsSessionPresent}", eventArgs.ConnectResult.IsSessionPresent ? "Yes" : "No");
        Logger.LogDebug("  Wildcard Subscription Available ..... {WildcardSubscriptionAvailable}", eventArgs.ConnectResult.WildcardSubscriptionAvailable ? "Yes" : "No");
        Logger.LogDebug("  Retain Available .................... {RetainAvailable}", eventArgs.ConnectResult.RetainAvailable ? "Yes" : "No");
        Logger.LogDebug("  Assigned Client Identifier .......... {AssignedClientIdentifier}", eventArgs.ConnectResult.AssignedClientIdentifier);
        Logger.LogDebug("  Authentication Method ............... {AuthenticationMethod}", eventArgs.ConnectResult.AuthenticationMethod);
        Logger.LogDebug("  Maximum Packet Size ................. {MaximumPacketSize}", eventArgs.ConnectResult.MaximumPacketSize);
        Logger.LogDebug("  Reason .............................. {Reason}", eventArgs.ConnectResult.ReasonString);
        Logger.LogDebug("  Receive Maximum ..................... {ReceiveMaximum}", eventArgs.ConnectResult.ReceiveMaximum);
        Logger.LogDebug("  Maximum QoS ......................... {MaximumQoS}", eventArgs.ConnectResult.MaximumQoS);
        Logger.LogDebug("  Response Information ................ {ReceiveMaximum}", eventArgs.ConnectResult.ResponseInformation);
        Logger.LogDebug("  Topic Alias Maximum ................. {TopicAliasMaximum}", eventArgs.ConnectResult.TopicAliasMaximum == 0 ? "Not supported" : eventArgs.ConnectResult.TopicAliasMaximum);
        Logger.LogDebug("  Server Reference .................... {ServerReference}", eventArgs.ConnectResult.ServerReference);
        Logger.LogDebug("  Server Keep Alive ................... {ServerKeepAlive}", eventArgs.ConnectResult.ServerKeepAlive);
        Logger.LogDebug("  Session Expiry Interval ............. {SessionExpiryInterval}", eventArgs.ConnectResult.SessionExpiryInterval != null ? eventArgs.ConnectResult.SessionExpiryInterval.Value.ToString(CultureInfo.InvariantCulture) : "N/A");
        Logger.LogDebug("  Subscription Identifiers Available .. {SubscriptionIdentifiersAvailable}", eventArgs.ConnectResult.SubscriptionIdentifiersAvailable ? "Yes" : "No");
        Logger.LogDebug("  Shared Subscription Available ....... {SharedSubscriptionAvailable}", eventArgs.ConnectResult.SharedSubscriptionAvailable ? "Yes" : "No");
        Logger.LogDebug("  User Properties ..................... {HasUserProperties}", eventArgs.ConnectResult.UserProperties?.Count > 0 ? string.Empty : "N/A");

        if (eventArgs.ConnectResult.UserProperties?.Count > 0)
        {
            foreach (var userProperty in eventArgs.ConnectResult.UserProperties)
            {
                Logger.LogDebug("    {Name}: {Value}", userProperty.Name, userProperty.Value);
            }
        }

        return Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private async Task OnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
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
            success = await application.RequestDelegate.Invoke(context);

            if (!success)
            {
                Logger.LogWarning("No handler found for message on {Topic}", context.Topic);
            }

            eventArgs.ProcessingFailed = !success;
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "Request failed to process: {Message}", exc.Message);
        }

        eventArgs.ProcessingFailed = !success;
    }

    [ExcludeFromCodeCoverage]
    private Task OnApplicationMessageProcessedAsync(ApplicationMessageProcessedEventArgs eventArgs)
    {
        Logger.LogDebug("Request processed.");
        return Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private Task OnApplicationMessageSkippedAsync(ApplicationMessageSkippedEventArgs eventArgs)
    {
        Logger.LogWarning("Request skipped.");
        return Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private async Task UnsubscribeTopicsAsync(IMqttTopicFilter[]? previousTopicFilters)
    {
        if (previousTopicFilters == null)
        {
            return;
        }

        var topics = previousTopicFilters.Select(x => x.Topic).ToArray();
        await MqttClient.UnsubscribeAsync(topics);
    }

    [ExcludeFromCodeCoverage]
    private async Task SubscribeToTopicsAsync(IMqttTopicFilter[] currentTopicFilters)
    {
        if (currentTopicFilters.Length == 0)
        {
            return;
        }

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

        await MqttClient.SubscribeAsync(currentTopicFilters.Select(x => x.ToMqttNetTopicFilter()).ToArray());
    }

    [ExcludeFromCodeCoverage]
    private void LogDisconnectEvent(MqttClientDisconnectedEventArgs eventArgs)
    {
        if (!IsShuttingDown && eventArgs.Reason != MqttClientDisconnectReason.NormalDisconnection)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (eventArgs.ClientWasConnected)
            {
                Logger.LogWarning("MQTT disconnected: {ReasonMessage} ({Reason})", eventArgs.ReasonString, eventArgs.Reason);
            }
            else
            {
                Logger.LogWarning("MQTT connect failed: {ReasonMessage} ({Reason})", eventArgs.ReasonString, eventArgs.Reason);
            }

            if (eventArgs.Exception != null)
            {
                Logger.LogError(eventArgs.Exception, "Error: {Message}", eventArgs.Exception.Message);
            }
        }
    }
}
