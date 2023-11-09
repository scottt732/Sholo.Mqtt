using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.DependencyInjection;
using Xunit;

namespace Sholo.Mqtt.Test;

public class MqttServiceCollectionTests
{
    [Fact]
    public void AddMqttApplicationPart_AddsMqttApplicationPartToServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        var mqttServiceCollection = new MqttServiceCollection(serviceCollection, "mqtt");
        var assembly = Assembly.GetExecutingAssembly();

        mqttServiceCollection.AddMqttApplicationPart(assembly);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var mqttApplicationPart = serviceProvider.GetService<MqttApplicationPart>();

        Assert.NotNull(mqttApplicationPart);
        Assert.Equal("mqtt", mqttServiceCollection.ConfigSectionPath);
        Assert.Equal(assembly, mqttApplicationPart.Assembly);
    }

    [Fact]
    public void AddMqttApplicationPart_Generic_AddsMqttApplicationPartToServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        var mqttServiceCollection = new MqttServiceCollection(serviceCollection, "mqtt");

        mqttServiceCollection.AddMqttApplicationPart<MqttServiceCollectionTests>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var mqttApplicationPart = serviceProvider.GetService<MqttApplicationPart>();

        Assert.NotNull(mqttApplicationPart);
        Assert.Equal("mqtt", mqttServiceCollection.ConfigSectionPath);
        Assert.Equal(typeof(MqttServiceCollectionTests).Assembly, mqttApplicationPart.Assembly);
    }
}
