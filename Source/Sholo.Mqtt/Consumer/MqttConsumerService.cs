using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;

namespace Sholo.Mqtt.Consumer
{
    public class MqttConsumerService<TManagedMqttSettings> : IHostedService
        where TManagedMqttSettings : ManagedMqttSettings, new()
    {
        protected IManagedMqttClientOptions MqttClientOptions { get; }
        protected IManagedMqttClient MqttClient { get; }
        protected IMqttApplication MqttApplication { get; }
        protected IOptions<TManagedMqttSettings> MqttSettings { get; }
        protected ILogger Logger { get; }

        public MqttConsumerService(
            IManagedMqttClientOptions mqttClientOptions,
            IManagedMqttClient mqttClient,
            IMqttApplication mqttApplication,
            IOptions<TManagedMqttSettings> mqttOptions,
            ILogger<MqttConsumerService<TManagedMqttSettings>> logger)
        {
            MqttClientOptions = mqttClientOptions;
            MqttClient = mqttClient;
            MqttApplication = mqttApplication;
            MqttSettings = mqttOptions;
            Logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(async e => await ProcessMessage(e));

            Logger.LogInformation("Subscribing to {topicCount} topics:", MqttApplication.TopicFilters.Count);
            foreach (var filter in MqttApplication.TopicFilters)
            {
                Logger.LogInformation(" - {filter} (QoS={qos})", filter.Topic, filter.QualityOfServiceLevel);
            }

            await MqttClient.SubscribeAsync(MqttApplication.TopicFilters);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await MqttClient.StopAsync();
        }

        private async Task ProcessMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var context = new MqttRequestContext(e.ApplicationMessage, e.ClientId);

                if (MqttApplication.RequestDelegate != null)
                {
                    var success = false;
                    try
                    {
                        success = await MqttApplication.RequestDelegate?.Invoke(context);

                        if (success)
                        {
                            Logger.LogDebug("Request processed in {duration}.", stopwatch.ElapsedMilliseconds.ToString("N0") + "ms");
                        }
                        else
                        {
                            Logger.LogWarning("Request did not match any subscribed patterns");
                        }
                    }
                    catch (Exception exc)
                    {
                        Logger.LogError(exc, "Request failed to process after {duration}.", stopwatch.ElapsedMilliseconds.ToString("N0") + "ms");
                    }

                    e.ProcessingFailed = !success;
                }
                else
                {
                    e.ProcessingFailed = true;
                }
            }
            catch (Exception exc)
            {
                Logger.LogError(exc, "  Unable to process request.  Ignoring: " + exc.Message);
            }
        }
    }
}
