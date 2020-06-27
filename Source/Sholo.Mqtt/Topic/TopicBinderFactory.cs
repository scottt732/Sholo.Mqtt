using System;
using JetBrains.Annotations;

namespace Sholo.Mqtt.Topic
{
    [PublicAPI]
    public static class TopicBinderFactory
    {
        public static ITopicBinderFactory<TModel> CreateDefault<TModel>()
            where TModel : class, new()
        {
            return TopicBinderFactoryBuilder.CreateDefault<TModel>().WithAutoProperties().BuildFactory();
        }

        public static ITopicBinderFactory<TModel> CreateDefault<TModel>(Func<TModel> targetFactory)
            where TModel : class
        {
            return TopicBinderFactoryBuilder.CreateDefault(targetFactory).WithAutoProperties().BuildFactory();
        }
    }
}
