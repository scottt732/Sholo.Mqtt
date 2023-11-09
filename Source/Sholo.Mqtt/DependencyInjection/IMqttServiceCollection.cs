using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.DependencyInjection;

[PublicAPI]
public interface IMqttServiceCollection : IServiceCollection
{
    string ConfigSectionPath { get; }

    IMqttServiceCollection AddMqttApplicationPart(Assembly assembly);

    IMqttServiceCollection AddMqttApplicationPart<TAssemblyClass>()
        where TAssemblyClass : class;
}
