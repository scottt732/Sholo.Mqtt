using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.TypeConverters.Parameter;

namespace Sholo.Mqtt.TypeConverters
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttParameterTypeConverter<TTargetType>(
            this IServiceCollection serviceCollection,
            Func<string, TTargetType> converter
        )
        {
            serviceCollection.AddSingleton(new LambdaMqttParameterTypeConverter<TTargetType>(converter));
            return serviceCollection;
        }

        public static IServiceCollection AddMqttParameterTypeConverter<TTypeConverter>(
            this IServiceCollection serviceCollection
        )
            where TTypeConverter : class, IMqttParameterTypeConverter
        {
            serviceCollection.AddSingleton<IMqttParameterTypeConverter, TTypeConverter>();
            return serviceCollection;
        }

        public static IServiceCollection AddMqttParameterTypeConverter(
            this IServiceCollection serviceCollection,
            IMqttParameterTypeConverter converter
        )
        {
            serviceCollection.AddSingleton(converter);
            return serviceCollection;
        }
    }
}
