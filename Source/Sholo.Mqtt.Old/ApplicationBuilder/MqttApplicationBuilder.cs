namespace Sholo.Mqtt.Old.ApplicationBuilder
{
    /*
    public class MqttApplicationBuilder : IMqttApplicationBuilder
    {
        public string PathPrefix { get; }

        private IDictionary<string, (IMqttTopicFilter topicFilter, MqttRequestDelegate requestDelegate)> TopicFilters { get; }

        public MqttApplicationBuilder()
        {
            TopicFilters = new Dictionary<string, (IMqttTopicFilter topicFilter, MqttRequestDelegate requestDelegate)>();
        }

        public MqttApplicationBuilder(string pathPrefix)
            : this()
        {
            PathPrefix = pathPrefix ?? string.Empty;
        }

        // 1
        public IMqttApplicationBuilder Map(
            IMqttTopicFilter topicFilter,
            MqttRequestDelegate requestDelegate)
        {
            if (topicFilter == null)
            {
                throw new ArgumentNullException(nameof(topicFilter));
            }

            if (requestDelegate == null)
            {
                throw new ArgumentNullException(nameof(requestDelegate));
            }

            async Task<bool> RequestDelegate(IMqttRequestContext context)
            {
                if (MqttTopicFilterComparer.IsMatch(context.Topic, topicFilter.Topic))
                {
                    return await requestDelegate.Invoke(context);
                }

                return false;
            }

            TopicFilters.Add(topicFilter.Topic, (topicFilter, RequestDelegate));

            return this;
        }

        // 2
        public IMqttApplicationBuilder Map<TTopicParameters>(
            IMqttTopicPatternFilter<TTopicParameters> topicPatternFilter,
            MqttRequestDelegate<TTopicParameters> requestDelegate)
                where TTopicParameters : class
        {
            if (topicPatternFilter == null)
            {
                throw new ArgumentNullException(nameof(topicPatternFilter));
            }

            if (requestDelegate == null)
            {
                throw new ArgumentNullException(nameof(requestDelegate));
            }

            return Map(
                topicPatternFilter,
                async context =>
                {
                    if (topicPatternFilter.IsMatch(context.Topic))
                    {
                        TTopicParameters parameters;

                        try
                        {
                            parameters = topicPatternFilter.Bind(context.Topic);
                        }
                        catch
                        {
                            return false;
                        }

                        var ctx = new MqttRequestContext<TTopicParameters>(context, parameters);
                        return await requestDelegate.Invoke(ctx);
                    }

                    return false;
                });
        }

        // 3
        public IMqttApplicationBuilder Use(MqttRequestDelegate requestDelegate)
        {
            if (requestDelegate == null)
            {
                throw new ArgumentNullException(nameof(requestDelegate));
            }

            // TODO: What about this.PathPrefix?
            var topicFilter = new Topics.FilterBuilder.MqttTopicFilterBuilder()
                .WithTopic("#")
                .Build();

            TopicFilters.Add(topicFilter.Topic, (topicFilter, requestDelegate));
            return this;
        }

        public IMqttApplication Build()
            => new MqttApplication(
                TopicFilters
                    .Select(x => new MqttTopicFilter
                    {
                        Topic = x.Value.topicFilter.Topic,
                        NoLocal = x.Value.topicFilter.NoLocal,
                        RetainAsPublished = x.Value.topicFilter.RetainAsPublished,
                        RetainHandling = x.Value.topicFilter.RetainHandling,
                        QualityOfServiceLevel = x.Value.topicFilter.QualityOfServiceLevel
                    }),
                BuildRequestDelegate()
            );

        private MqttRequestDelegate BuildRequestDelegate()
        {
            return async context =>
            {
                foreach (var component in TopicFilters)
                {
                    var topicFilter = component.Key;
                    var middleware = component.Value;

                    if (MqttTopicFilterComparer.IsMatch(context.Topic, topicFilter))
                    {
                        if (await middleware.requestDelegate.Invoke(context))
                        {
                            return true;
                        }
                    }
                }

                return false;
            };
        }
    }
    */
}
