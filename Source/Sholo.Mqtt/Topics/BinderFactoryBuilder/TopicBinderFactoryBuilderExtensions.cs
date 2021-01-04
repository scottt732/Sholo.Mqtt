using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Sholo.Mqtt.Topics.BinderFactoryBuilder
{
    [PublicAPI]
    public static class TopicBinderFactoryBuilderExtensions
    {
        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, bool>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, bool.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, char>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, char.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, decimal>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, decimal.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, double>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, double.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, float>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, float.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, int>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, int.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, uint>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, uint.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, long>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, long.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, ulong>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, ulong.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, short>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, short.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, ushort>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, ushort.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, Guid>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, Guid.Parse);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, bool?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? bool.Parse(s) : (bool?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, char?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? char.Parse(s) : (char?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, decimal?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? decimal.Parse(s) : (decimal?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, double?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? double.Parse(s) : (double?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, float?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? float.Parse(s) : (float?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, int?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? int.Parse(s) : (int?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, uint?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? uint.Parse(s) : (uint?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, long?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? long.Parse(s) : (long?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, ulong?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? ulong.Parse(s) : (ulong?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, short?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? short.Parse(s) : (short?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, ushort?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? ushort.Parse(s) : (ushort?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, Guid?>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, s => !string.IsNullOrEmpty(s) ? Guid.Parse(s) : (Guid?)null);

        public static ITopicBinderFactoryBuilder<TModel> WithProperty<TModel>(this ITopicBinderFactoryBuilder<TModel> topicBinderFactoryBuilder, Expression<Func<TModel, string>> expression)
            where TModel : class => topicBinderFactoryBuilder.WithProperty(expression, x => x);
    }
}
