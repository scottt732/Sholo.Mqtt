using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Mqtt.Sample.Services;

internal sealed class ClientService : BackgroundService
{
    private SortedDictionary<char, MenuItem> Items { get; }
    private IManagedMqttClient MqttClient { get; }
    private ILogger Logger { get; }

    public ClientService(
        IManagedMqttClient mqttClient,
        ILogger<ClientService> logger
    )
    {
        MqttClient = mqttClient;
        Logger = logger;

        Items = new SortedDictionary<char, MenuItem>(Comparer<char>.Create((a, b) => char.ToLowerInvariant(a).CompareTo(char.ToLowerInvariant(b))))
        {
            ['0'] = new("Send a message", ct => SendMessage(
                b => b
                    .WithTopic("test/run/scott/26.2")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                    .WithPayload("this is a test")
                    .WithRetainFlag(false),
                ct
            )),
            ['1'] = new("Send a message", ct => SendMessage(
                b => b
                    .WithTopic("test/run/scott/25/2")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                    .WithPayload("{\"hello\":\"world\",\"test\":\"123\"}"u8.ToArray())
                    .WithRetainFlag(false),
                ct
            )),
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        async Task Quit(CancellationTokenSource c)
        {
            await c.CancelAsync();
        }

        await WriteMenu(false);

        var cancellationToken = cts.Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Choose an option (? for menu, q to quit):");
            var c = Console.ReadKey(true);
            var lc = char.ToLowerInvariant(c.KeyChar);

            var task = lc switch
            {
                '?' => WriteMenu(false),
                'q' => Quit(cts),
                _ => Items.TryGetValue(lc, out var item) ? item.Action.Invoke(cancellationToken) : WriteMenu(true)
            };

            await task;
        }
    }

    private async Task SendMessage(Action<MqttApplicationMessageBuilder> configuration, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Sending a message");

        var mqttApplicationMessageBuilder = new MqttApplicationMessageBuilder();

        configuration.Invoke(mqttApplicationMessageBuilder);

        var mqttApplicationMessage = mqttApplicationMessageBuilder.Build();

        await MqttClient.InternalClient.PublishAsync(mqttApplicationMessage, cancellationToken);
    }

    private Task WriteMenu(bool showErrorMessage)
    {
        if (showErrorMessage)
        {
            Console.WriteLine("Invalid option");
            Console.WriteLine();
        }

        foreach (var (c, item) in Items)
        {
            if (c is '?' or 'q')
            {
                throw new InvalidOperationException("? and q are reserved");
            }

            Console.WriteLine($"  {c}  {item.Description}");
        }

        Console.WriteLine("  ?  Help");
        Console.WriteLine("  q  Quit");

        return Task.CompletedTask;
    }

    private sealed class MenuItem
    {
        public string Description { get; }
        public Func<CancellationToken, Task> Action { get; }

        public MenuItem(string description, Func<CancellationToken, Task> action)
        {
            Description = description;
            Action = action;
        }
    }
}
