using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mqtt.Sample.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sholo.Mqtt;
using Sholo.Mqtt.Application.Builder;
using Sholo.Mqtt.Hosting;
using Sholo.Mqtt.TypeConverters.NewtonsoftJson;

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
            .ConfigureLogging(lb =>
            {
                lb.AddSimpleConsole(opt => { opt.SingleLine = true; });
            })
            .ConfigureServices((_, services) =>
            {
                services.AddMqttConsumerService("mqtt")
                    .AddNewtonsoftJsonTypeConverter(opt =>
                    {
                        opt.Encoding = Encoding.UTF8;
                        opt.SerializerSettings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        };
                    });

                services.AddHostedService<FakeClientService>();
            })
            .ConfigureMqttHost(app =>
            {
                // This enables automatic resolution of controllers from any loaded assemblies that have the `MqttApplicationPart` attribute.
                // In this example, the attribute is added via <AssemblyAttribute Include="MqttApplicationPart" /> in Mqtt.Sample.csproj.
                app.UseRouting();
            })
            .UseConsoleLifetime(opt =>
            {
                opt.SuppressStatusMessages = true;
            })
            .Build()
            .RunAsync();
    }
}
