using JetBrains.Annotations;

namespace Sholo.Mqtt.Old.ApplicationBuilder
{
    [PublicAPI]
    public static class MqttApplicationBuilderExtensions
    {
        /*
        // 1a is in the interface, b-d
        public static IMqttApplicationBuilder Map(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicFilterBuilder> topicConfiguration,
            MqttRequestDelegate requestDelegate)
        {
            var topicFilter = CreateTopicFilter(topicConfiguration);

            return mqttApplicationBuilder.Map(topicFilter, requestDelegate);
        }

        public static IMqttApplicationBuilder Map(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicFilter topicFilter,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
        {
            var requestDelegate = CreateRequestDelegate(configureRequestDelegate, mqttApplicationBuilder.PathPrefix);

            return mqttApplicationBuilder.Map(topicFilter, requestDelegate);
        }

        public static IMqttApplicationBuilder Map(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicFilterBuilder> topicConfiguration,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
        {
            var topicFilter = CreateTopicFilter(topicConfiguration);
            var requestDelegate = CreateRequestDelegate(configureRequestDelegate, mqttApplicationBuilder.PathPrefix);

            return mqttApplicationBuilder.Map(topicFilter, requestDelegate);
        }

        // 2a is in the interface, b-d
        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicPatternFilterBuilder<TTopicParameters>> topicConfiguration,
            MqttRequestDelegate<TTopicParameters> requestDelegate)
            where TTopicParameters : class
        {
            var topicPatternFilter = CreateTopicPatternFilter(topicConfiguration);

            return mqttApplicationBuilder.Map(topicPatternFilter, requestDelegate);
        }

        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicPatternFilter<TTopicParameters> topicPatternFilter,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
            where TTopicParameters : class
        {
            var requestDelegate = CreateRequestDelegate(configureRequestDelegate, mqttApplicationBuilder.PathPrefix);

            return mqttApplicationBuilder
                .Map(topicPatternFilter, requestDelegate);
        }

        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicPatternFilterBuilder<TTopicParameters>> topicConfiguration,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
            where TTopicParameters : class
        {
            var topicPatternFilter = CreateTopicPatternFilter(topicConfiguration);
            var requestDelegate = CreateRequestDelegate(configureRequestDelegate, mqttApplicationBuilder.PathPrefix);

            return mqttApplicationBuilder
                .Map(topicPatternFilter, requestDelegate);
        }

        // 3a is in the interface, b only
        public static IMqttApplicationBuilder Use(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
        {
            var requestDelegate = CreateRequestDelegate(configureRequestDelegate, mqttApplicationBuilder.PathPrefix);

            return mqttApplicationBuilder
                .Use(requestDelegate);
        }

        // 2a-d
        public static IMqttApplicationBuilder MapTyped<TPayload>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicFilter topicFilter,
            MqttRequestDelegate requestDelegate)
        {
            throw new NotImplementedException();
        }

        public static IMqttApplicationBuilder MapTyped<TPayload>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicFilterBuilder> topicConfiguration,
            MqttRequestDelegate requestDelegate)
        {
            var topicFilter = CreateTopicFilter(topicConfiguration);

            return mqttApplicationBuilder.Map(topicFilter, requestDelegate);
        }

        public static IMqttApplicationBuilder MapTyped<TPayload>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicFilter topicFilter,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
        {
            var requestDelegate = CreateRequestDelegate(configureRequestDelegate, mqttApplicationBuilder.PathPrefix);

            return mqttApplicationBuilder.Map(topicFilter, requestDelegate);
        }

        public static IMqttApplicationBuilder MapTyped<TPayload>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicFilterBuilder> topicConfiguration,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
        {
            var topicFilter = CreateTopicFilter(topicConfiguration);
            var requestDelegate = CreateRequestDelegate(configureRequestDelegate, mqttApplicationBuilder.PathPrefix);

            return mqttApplicationBuilder.Map(topicFilter, requestDelegate);
        }

        // Helpers
        private static IMqttTopicFilter CreateTopicFilter(Action<IMqttTopicFilterBuilder> topicConfiguration)
        {
            var topicFilterBuilder = new MqttTopicFilterBuilder();
            topicConfiguration?.Invoke(topicFilterBuilder);
            var topicFilter = topicFilterBuilder.Build();
            return topicFilter;
        }

        private static IMqttTopicPatternFilter<TTopicParameters> CreateTopicPatternFilter<TTopicParameters>(
            Action<IMqttTopicPatternFilterBuilder<TTopicParameters>> topicConfiguration
        )
            where TTopicParameters : class
        {
            var topicPatternFilterBuilder = new MqttTopicPatternFilterBuilder<TTopicParameters>();
            topicConfiguration?.Invoke(topicPatternFilterBuilder);
            var topicPatternFilter = topicPatternFilterBuilder.Build();
            return topicPatternFilter;
        }

        private static MqttRequestDelegate CreateRequestDelegate(Action<IMqttApplicationBuilder> configureRequestDelegate, string pathPrefix)
        {
            var mappedMqttApplicationBuilder = new MqttApplicationBuilder(pathPrefix);
            configureRequestDelegate?.Invoke(mappedMqttApplicationBuilder);
            var mappedMqttApplication = mappedMqttApplicationBuilder.Build();
            var requestDelegate = mappedMqttApplication.RequestDelegate;
            return requestDelegate;
        }


        private static MqttRequestDelegate CreateRequestDelegate<TPayload>(
            Action<IMqttApplicationBuilder> configureRequestDelegate,
            string pathPrefix)
        {
            var mappedMqttApplicationBuilder = new MqttApplicationBuilder(pathPrefix);
            configureRequestDelegate?.Invoke(mappedMqttApplicationBuilder);
            var mappedMqttApplication = mappedMqttApplicationBuilder.Build();
            var requestDelegate = mappedMqttApplication.RequestDelegate;
            return requestDelegate;
        }

        private static MqttRequestDelegate CreateRequestDelegate<TTopicParameters, TPayload>(
            Action<IMqttApplicationBuilder> configureRequestDelegate,
            string pathPrefix)
        {
            var mappedMqttApplicationBuilder = new MqttApplicationBuilder(pathPrefix);
            configureRequestDelegate?.Invoke(mappedMqttApplicationBuilder);
            var mappedMqttApplication = mappedMqttApplicationBuilder.Build();
            var requestDelegate = mappedMqttApplication.RequestDelegate;
            return requestDelegate;
        }
        */

        /*
        // 1a is IMqttApplicationBuilder defined
        // 1b - Action<IMqttTopicFilterBuilder configureTopicFilter
        public static IMqttApplicationBuilder Map(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicFilterBuilder> configureTopicFilter,
            MqttRequestDelegate requestDelegate)
        {
            var topicFilterBuilder = new MqttTopicFilterBuilder();
            configureTopicFilter?.Invoke(topicFilterBuilder);

            var topicFilter = topicFilterBuilder.Build();

            return mqttApplicationBuilder
                .Map(topicFilter, requestDelegate);
        }

        // 1c
        public static IMqttApplicationBuilder Map(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicFilter topicFilter,
            Action<IMqttApplicationBuilder> configureRequestDelegate)
        {
            var mappedMqttApplicationBuilder = new MqttApplicationBuilder();
            configureRequestDelegate?.Invoke(mappedMqttApplicationBuilder);

            var mappedMqttApplication = mappedMqttApplicationBuilder.Build();
            var mappedRequestDelegate = mappedMqttApplication.RequestDelegate;

            return mqttApplicationBuilder
                .Map(topicFilter, mappedRequestDelegate);
        }

        // 2a is IMqttApplicationBuilder defined
        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicPatternFilter topicFilter,
            MqttRequestDelegate<TTopicParameters> requestDelegate,
            Func<TTopicParameters, bool> parametersPredicate = null,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null)
            where TTopicParameters : class, new()
        { }
        */

        // 1d

        /*

        // 2
        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicPatternFilter topicFilter,
            MqttRequestDelegate<TTopicParameters> requestDelegate,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null)
                where TTopicParameters : class, new()

        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicPatternFilterBuilder> configureTopicFilter,
            MqttRequestDelegate<TTopicParameters> requestDelegate,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null)
                where TTopicParameters : class, new()

        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicPatternFilter topicFilter,
            Action<IMqttApplicationBuilder> configureRequestDelegate,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null)
                where TTopicParameters : class, new()

        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicPatternFilterBuilder> configureTopicFilter,
            Action<IMqttApplicationBuilder> configureRequestDelegate,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null)
                where TTopicParameters : class, new()
        */

        /*
        public static IMqttApplicationBuilder UseProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder
        )
            where TProcessor : IMqttRequestProcessor
        {
            mqttApplicationBuilder.Use(async context =>
            {
                var processor = context.ServiceProvider.GetRequiredService<TProcessor>();
                return await processor.ProcessRequest(context);
            });

            return mqttApplicationBuilder;
        }

        public static IMqttApplicationBuilder MapProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicFilter topicFilter
        )
            where TProcessor : IMqttRequestProcessor
        {
            return mqttApplicationBuilder.Map(
                topicFilter,
                async context =>
                {
                    var processor = context.ServiceProvider.GetRequiredService<TProcessor>();
                    return await processor.ProcessRequest(context);
                }
            );
        }
        */

        /*
        public static IMqttApplicationBuilder MapProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicFilterBuilder> configureTopicFilter
        )
            where TProcessor : IMqttRequestProcessor
         */

        /*
        public static IMqttApplicationBuilder MapProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicFilter topicFilter,
            TProcessor processor
        )
            where TProcessor : IMqttRequestProcessor
        {
            return mqttApplicationBuilder.Map(
                topicFilter,
                async context => await processor.ProcessRequest(context)
            );
        }
        */

        /*
        public static IMqttApplicationBuilder MapProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicFilterBuilder> configureTopicFilter,
            TProcessor processor
        )
            where TProcessor : IMqttRequestProcessor
        {
            return mqttApplicationBuilder.Map(
                topicFilter,
                async context => await processor.ProcessRequest(context)
            );
        }
         */

        /*
        public static IMqttApplicationBuilder MapProcessor<TProcessor, TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            IMqttTopicPatternFilter topicFilter,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null)
                where TProcessor : IMqttRequestProcessor<TTopicParameters>
                where TTopicParameters : class, new()
        {
            var topicBinder = TopicBinderHelper.CreateTopicBinder(topicFilter.TopicPattern, topicBinderFactoryBuilderConfigurator);

            return mqttApplicationBuilder.Map(
                topicFilter,
                async context =>
                {
                    if (topicBinder.IsMatch(context.Topic))
                    {
                        TTopicParameters parameters;

                        try
                        {
                            parameters = topicBinder.Bind(context.Topic);
                        }
                        catch
                        {
                            return false;
                        }

                        var parameterizedContext = new MqttRequestContext<TTopicParameters>(context, parameters);
                        var processor = context.ServiceProvider.GetRequiredService<TProcessor>();

                        return await processor.ProcessRequest(parameterizedContext);
                    }

                    return false;
                });
        }
        */

        /*

        T
        O
        D
        O


        T
        O
        D
        O

        public static IMqttApplicationBuilder MapProcessor<TProcessor, TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            Action<IMqttTopicPatternFilterBuilder> configureTopicFilter,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null)
                where TProcessor : IMqttRequestProcessor<TTopicParameters>
                where TTopicParameters : class, new()

        T
        O
        D
        O


        T
        O
        D
        O
        */
    }
}
