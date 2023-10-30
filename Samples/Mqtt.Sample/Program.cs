using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mqtt.Sample;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("appsettings.json");
            })
            .ConfigureLogging(lb => { lb.AddSimpleConsole(opt => { opt.SingleLine = true; }); })
            .ConfigureServices((_, services) =>
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
            .ConfigureMqttHost(app => { app.UseRouting(); })
            .UseConsoleLifetime(opt => { opt.SuppressStatusMessages = true; })
            .Build()
            .RunAsync()
            ;
    }
}
