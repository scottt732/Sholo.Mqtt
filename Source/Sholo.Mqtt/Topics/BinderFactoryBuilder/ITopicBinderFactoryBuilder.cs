using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Sholo.Mqtt.Topics.BinderFactory;

namespace Sholo.Mqtt.Topics.BinderFactoryBuilder
{
    [PublicAPI]
    public interface ITopicBinderFactoryBuilder<TModel>
        where TModel : class
    {
        ITopicBinderFactoryBuilder<TModel> WithTypeConverter<TProperty>(Func<string, TProperty> propertyConverter);
        ITopicBinderFactoryBuilder<TModel> WithTypeConverterFor<TProperty>(Expression<Func<TModel, TProperty>> propertySelector, Func<string, TProperty> propertyConverter);
        ITopicBinderFactoryBuilder<TModel> WithAutoProperties();
        ITopicBinderFactoryBuilder<TModel> WithProperty<TProperty>(Expression<Func<TModel, TProperty>> propertySelector, Func<string, TProperty> typeConverter = null);
        ITopicBinderFactory<TModel> BuildFactory();
    }
}
