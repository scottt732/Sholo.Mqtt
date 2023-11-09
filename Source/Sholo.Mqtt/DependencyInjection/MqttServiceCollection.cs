using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.Utilities;

namespace Sholo.Mqtt.DependencyInjection;

internal class MqttServiceCollection : BaseServiceCollectionExtender, IMqttServiceCollection
{
    public string ConfigSectionPath { get; }

    public MqttServiceCollection(IServiceCollection target, string configSectionPath)
        : base(target)
    {
        ConfigSectionPath = configSectionPath;
    }

    public IMqttServiceCollection AddMqttApplicationPart(Assembly assembly)
    {
        this.AddSingleton(new MqttApplicationPart(assembly));
        return this;
    }

    public IMqttServiceCollection AddMqttApplicationPart<TAssemblyClass>()
        where TAssemblyClass : class
    {
        return AddMqttApplicationPart(typeof(TAssemblyClass).Assembly);
    }
}
