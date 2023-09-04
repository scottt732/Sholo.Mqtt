using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.DependencyInjection
{
    [PublicAPI]
    public interface IMqttServiceCollection : IServiceCollection
    {
        IMqttServiceCollection AddMqttApplicationPart(Assembly assembly);

        IMqttServiceCollection AddMqttApplicationPart<TAssemblyClass>()
            where TAssemblyClass : class;
    }
}
