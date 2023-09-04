using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(cb =>
    {
        cb.AddUserSecrets<Mqtt.Sample.TestController>();
    })
    .ConfigureLogging(lb =>
    {
        lb.AddSimpleConsole(opt =>
        {
            opt.SingleLine = true;
        });
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddMqttConsumerService("mqtt")
            .AddNewtonsoftJsonPayloadConverter(opt =>
            {
                opt.Encoding = Encoding.UTF8;
                opt.SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            });
    })
    .ConfigureMqttHost(app =>
    {
        app.UseRouting();
    })
    .UseConsoleLifetime(opt =>
    {
        opt.SuppressStatusMessages = true;
    })
    .Build()
    .RunAsync();
