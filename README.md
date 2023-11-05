![Banner](Images/Banner.png)

# Sholo.Mqtt

[![Sholo.Mqtt NuGet Package](https://img.shields.io/nuget/v/Sholo.Mqtt.svg)](https://www.nuget.org/packages/Sholo.Mqtt/)
[![Twitter URL](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/scottt732)
[![Twitter Follow](https://img.shields.io/twitter/follow/scottt732.svg?style=social&label=Follow)](https://twitter.com/scottt732)

Sholo.MQTT is a lightweight ASP.NET Core-inspired framework for applications that consume and/or produce
MQTT messages.  It bridges [chkr1011/MQTTnet](https://github.com/chkr1011/MQTTnet) with the .NET Generic Host,
provides an [`IApplicationBuilder`-like pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0)
for configuring your MQTT subscriptions, and provides strongly-typed, validated databinding of MQTT payload and topic segments (similar to
ASP.NET Core's MVC's model binding).

## Example

### Program.cs

```c#
public static async Task Main(string[] args)
    => await Host.CreateDefaultBuilder(args)
        .ConfigureServices((ctx, services) =>
        {
            services.AddMqttConsumerService(
                ctx.Configuration.GetSection("mqtt"),
                builder => {
                    builder.MapProcessor<TestTypedProcessor, TestParameters>(

                        // Define variables One, Two, Three
                        "mytopic/test/+One/+Two/+Three",

                        // Apply a predicate for this processor allowing multiple
                        // processors to share a topic pattern (optional)
                        p => p.One % 2 == 0 && p.Two == true && p.Three == "go",

                        cfg => cfg.WithAutoProperties(),
                        MqttQualityOfServiceLevel.AtLeastOnce);

                    builder.MapProcessor<TestUntypedProcessor>(
                        "mytopic/test2",
                        MqttQualityOfServiceLevel.AtLeastOnce);
                }
            );
        })
        .Build()
        .RunAsync();
```

### TestTypedProcessor.cs

```c#
public class TestTypedProcessor : IMqttRequestProcessor<TestParameters>
{
    private ILogger Logger { get; }

    public TestProcessor(ILogger<TestTypedProcessor> logger)
    {
        // Inject whatever you need/registered
        // A ServiceScope will be created to service this request so your
        // Scoped registrations will work as expected (one instance per
        // MQTT message).
        Logger = logger;
    }

    public Task<bool> ProcessRequest(MqttRequestContext<TestParameters> context)
    {
        Logger.LogInformation($"{context.TopicParameters.One}");
        Logger.LogInformation($"{context.TopicParameters.Two}");
        Logger.LogInformation($"{context.TopicParameters.Three}");
        Logger.LogInformation(context.ConvertPayloadToString());

        return Task.FromResult(true);
    }
}
```

### TestParameters.cs

```c#
public class TestParameters
{
    public int One { get; set; }
    public bool Two { get; set; }
    public string Three { get; set; }
}
```

### TestUntypedProcessor.cs

```c#
public class TestUntypedProcessor : IMqttRequestProcessor
{
    public IManagedMqttClient ManagedMqttClient { get; }

    public TestUntypedProcessor(IManagedMqttClient managedMqttClient)
    {
        ManagedMqttClient = managedMqttClient;
    }

    public async Task<bool> ProcessRequest(MqttRequestContext context)
    {
        Logger.LogInformation("Got a message");

        await Client.PublishAsync(
            new MqttApplicationMessageBuilder()
                .WithTopic("some/other/app")
                .WithPayload("Hello, world")
                .Build()
        );
    }
}
```

## Behavior

The builder configuration in the `AddMqttConsumerService` builds a pipeline of
processors which are evaluated in order for each subscribed MQTT message.
The first processor to return true will break out of the loop.  This allows higher
processors to observe messages.

The pattern matching code attempts to compute and subscribe to a minimal set of
topic patterns that satisfy your application's requirements.  It will ignore/not
dispatch messages when no registered patterns match.

While the `IMqttRequestProcessor`s provide you with a higher-level abstraction for
consuming messages, you can also provide something closer to ASP.NET Core middleware:

```c#
public delegate Task<bool> MqttRequestDelegate(MqttRequestContext context);

public delegate Task<bool> MqttRequestDelegate<TParameters>(MqttRequestContext<TParameters> context)
    where TParameters : class, new();
```

The `MqttRequestContext` class is a simple extension of **chkr1011/MQTTnet**'s [MqttApplicationMessage](https://github.com/chkr1011/MQTTnet/blob/master/Source/MQTTnet/MqttApplicationMessage.cs#L7)
with `ClientId` and (optionally) the strongly typed topic parameters.

## Thanks

This project is based on [@chkratky](https://twitter.com/chkratky)'s [MQTTnet library](https://github.com/chkr1011/MQTTnet).  Thanks to Christian and all
of the contributors on that project!

A huge thanks to [@RahenSaeedUK](https://twitter.com/RehanSaeedUK) for the [.NET Boxed Templates](https://github.com/Dotnet-Boxed/Templates), without which I probably
would have never gotten around to releasing any of this.

## Related Projects

I've been building a bunch of small utilities, applications, and container related to home automation,
primarily using MQTT and/or [Home Assistant](https://www.home-assistant.io/).

There are a few foundational libraries/frameworks that I've created along the way.  The table shows the third-party pieces
they're built on.

<table width="100%">
    <tr>
        <th colspan="4">Home Automation Foundation Libraries</th>
    </tr>
    <tr>
        <th width="25%"><a href="https://github.com/scottt732/Sholo.CommandLine">Sholo.CommandLine</a></td>
        <th width="25%"><a href="https://github.com/scottt732/Sholo.HomeAssistant">Sholo.HomeAssistant</a></td>
        <th width="25%"><a href="https://github.com/scottt732/Sholo.Mqtt">Sholo.Mqtt</a></td>
        <th width="25%"><a href="https://github.com/scottt732/Sholo.Utils">Sholo.Utils</a></td>
    </tr>
    <tr>
        <th width="25%"><a href="https://github.com/natemcmaster/CommandLineUtils">natemcmaster/CommandLineUtils</a></td>
        <th width="25%">-</td>
        <th width="25%"><a href="https://github.com/chkr1011/MQTTnet">chkr1011/MQTTnet</a></td>
        <th width="25%">-</td>
    </tr>
</table>

And a handful of applications that I plan to publish soon.

## License

MIT License

Copyright (c) 2020 Scott Holodak

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
