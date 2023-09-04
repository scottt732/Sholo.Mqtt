using System;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.Old.Utilities;

namespace Sholo.Mqtt.Old.DependencyInjection
{
    internal class MqttServiceCollection : BaseServiceCollectionExtender, IMqttServiceCollection
    {
        public MqttServiceCollection(IServiceCollection target)
            : base(target)
        {
        }

        public IMqttServiceCollection AddTopicBindingConfiguration<TTopicParameters>()
            where TTopicParameters : class
        {
            throw new NotImplementedException();
        }
    }
}
