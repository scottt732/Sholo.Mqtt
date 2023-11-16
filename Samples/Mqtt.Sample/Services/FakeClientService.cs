using Microsoft.Extensions.Hosting;

namespace Mqtt.Sample.Services;

internal class FakeClientService : BackgroundService
{
    private SortedDictionary<char, MenuItem> Items { get; } = new(Comparer<char>.Create((a, b) => char.ToLowerInvariant(a).CompareTo(char.ToLowerInvariant(b))))
    {
        ['c'] = new MenuItem("c", (ct) =>
        {
            Console.WriteLine("you pressed c");
            return Task.CompletedTask;
        })
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        Task Quit(CancellationTokenSource c)
        {
            c.Cancel();
            return Task.CompletedTask;
        }

        await WriteMenu();

        while (!cts.Token.IsCancellationRequested)
        {
            Console.WriteLine("Choose an option (? for menu, q to quit):");
            var c = Console.ReadKey(true);
            var lc = char.ToLowerInvariant(c.KeyChar);

            var task = lc switch
            {
                '?' => WriteMenu(),
                'q' => Quit(cts),
                _ => WriteMenu()
            };

            await task;
        }
    }

    private Task Handle(char c)
    {

    }

    private Task WriteMenu()
    {
        foreach (var (c, item) in Items)
        {
            Console.WriteLine($"  {c}  {item.Description}");
        }

        return Task.CompletedTask;
    }

    public class MenuItem
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
