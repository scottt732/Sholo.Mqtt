using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.Utilities;

namespace Sholo.Mqtt.DependencyInjection;

internal class MqttServiceCollection : BaseServiceCollectionExtender, IMqttServiceCollection
{
    public MqttServiceCollection(IServiceCollection target)
        : base(target)
    {
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
