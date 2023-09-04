using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.Old.DependencyInjection
{
    public interface IMqttServiceCollection : IServiceCollection
    {
        IMqttServiceCollection AddTopicBindingConfiguration<TTopicParameters>()
            where TTopicParameters : class;
    }
}
