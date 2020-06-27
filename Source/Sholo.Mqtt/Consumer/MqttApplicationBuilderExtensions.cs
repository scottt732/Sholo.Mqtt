using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;
using Sholo.Mqtt.Topic;

namespace Sholo.Mqtt.Consumer
{
    [PublicAPI]
    public static class MqttApplicationBuilderExtensions
    {
        public static IMqttApplicationBuilder UseDefault(this IMqttApplicationBuilder mqttApplicationBuilder)
        {
            var logger = mqttApplicationBuilder.ApplicationServices.GetService<ILogger<IMqttApplicationBuilder>>();
            mqttApplicationBuilder.Use(context =>
            {
                logger?.LogInformation("Invoking default handler");
                return Task.FromResult(true);
            });
            return mqttApplicationBuilder;
        }

        public static IMqttApplicationBuilder UseProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder
        )
            where TProcessor : IMqttRequestProcessor
        {
            mqttApplicationBuilder.Use(async context =>
            {
                var serviceScopeFactory = mqttApplicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var serviceScope = serviceScopeFactory.CreateScope())
                {
                    var processor = serviceScope.ServiceProvider.GetRequiredService<TProcessor>();
                    return await processor.ProcessRequest(context);
                }
            });

            return mqttApplicationBuilder;
        }

        public static IMqttApplicationBuilder Map<TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            string topicPattern,
            MqttRequestDelegate<TTopicParameters> middleware,
            Func<TTopicParameters, bool> parametersPredicate = null,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null,
            MqttQualityOfServiceLevel? qualityOfServiceLevel = null,
            bool? noLocal = null,
            bool? retainAsPublished = null,
            MqttRetainHandling? retainHandling = null)
                where TTopicParameters : class, new()
        {
            var topicBinder = CreateTopicBinder(topicPattern, topicBinderFactoryBuilderConfigurator);

            return mqttApplicationBuilder.Map(
                topicPattern,
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

                        if (parametersPredicate?.Invoke(parameters) ?? true)
                        {
                            var ctx = new MqttRequestContext<TTopicParameters>(context, parameters);
                            return await middleware.Invoke(ctx);
                        }
                    }

                    return false;
                },
                qualityOfServiceLevel,
                noLocal,
                retainAsPublished,
                retainHandling);
        }

        public static IMqttApplicationBuilder MapProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            string topicPattern,
            MqttQualityOfServiceLevel? qualityOfServiceLevel = null,
            bool? noLocal = null,
            bool? retainAsPublished = null,
            MqttRetainHandling? retainHandling = null
        )
            where TProcessor : IMqttRequestProcessor
        {
            return mqttApplicationBuilder.Map(
                topicPattern,
                async context =>
                {
                    var serviceScopeFactory = mqttApplicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                    using (var serviceScope = serviceScopeFactory.CreateScope())
                    {
                        var processor = serviceScope.ServiceProvider.GetRequiredService<TProcessor>();
                        return await processor.ProcessRequest(context);
                    }
                },
                qualityOfServiceLevel,
                noLocal,
                retainAsPublished,
                retainHandling);
        }

        public static IMqttApplicationBuilder MapProcessor<TProcessor>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            string topicPattern,
            TProcessor processor,
            MqttQualityOfServiceLevel? qualityOfServiceLevel = null,
            bool? noLocal = null,
            bool? retainAsPublished = null,
            MqttRetainHandling? retainHandling = null
        )
            where TProcessor : IMqttRequestProcessor
        {
            return mqttApplicationBuilder.Map(
                topicPattern,
                async context =>
                {
                    // TODO: Test if serviceScopeFactory is necessary
                    var serviceScopeFactory = mqttApplicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

                    // ReSharper disable once UnusedVariable
                    using (var serviceScope = serviceScopeFactory.CreateScope())
                    {
                        return await processor.ProcessRequest(context);
                    }
                },
                qualityOfServiceLevel,
                noLocal,
                retainAsPublished,
                retainHandling);
        }

        public static IMqttApplicationBuilder MapProcessor<TProcessor, TTopicParameters>(
            this IMqttApplicationBuilder mqttApplicationBuilder,
            string topicPattern,
            Func<TTopicParameters, bool> parametersPredicate = null,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator = null,
            MqttQualityOfServiceLevel? qualityOfServiceLevel = null,
            bool? noLocal = null,
            bool? retainAsPublished = null,
            MqttRetainHandling? retainHandling = null)
                where TProcessor : IMqttRequestProcessor<TTopicParameters>
                where TTopicParameters : class, new()
        {
            var topicBinder = CreateTopicBinder(topicPattern, topicBinderFactoryBuilderConfigurator);

            return mqttApplicationBuilder.Map(
                topicPattern,
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

                        if (parametersPredicate?.Invoke(parameters) ?? true)
                        {
                            var parameterizedContext = new MqttRequestContext<TTopicParameters>(context, parameters);

                            var serviceScopeFactory = mqttApplicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                            using (var serviceScope = serviceScopeFactory.CreateScope())
                            {
                                var processor = serviceScope.ServiceProvider.GetRequiredService<TProcessor>();
                                return await processor.ProcessRequest(parameterizedContext);
                            }
                        }
                    }

                    return false;
                },
                qualityOfServiceLevel,
                noLocal,
                retainAsPublished,
                retainHandling);
        }

        private static ITopicBinder<TTopicParameters> CreateTopicBinder<TTopicParameters>(
            string topicPattern,
            Action<ITopicBinderFactoryBuilder<TTopicParameters>> topicBinderFactoryBuilderConfigurator
        )
            where TTopicParameters : class, new()
        {
            ITopicBinderFactoryBuilder<TTopicParameters> topicBinderFactoryBuilder;
            if (topicBinderFactoryBuilderConfigurator != null)
            {
                topicBinderFactoryBuilder = TopicBinderFactoryBuilder.Create<TTopicParameters>();
                topicBinderFactoryBuilderConfigurator.Invoke(topicBinderFactoryBuilder);
            }
            else
            {
                topicBinderFactoryBuilder = TopicBinderFactoryBuilder.CreateDefault<TTopicParameters>();
            }

            var topicBinderFactory = topicBinderFactoryBuilder.BuildFactory();

            var topicBinder = topicBinderFactory.CreateBinder(topicPattern);
            return topicBinder;
        }
    }
}
