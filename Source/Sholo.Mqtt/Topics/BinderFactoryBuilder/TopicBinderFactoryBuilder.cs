using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Sholo.Mqtt.Topics.BinderFactory;

namespace Sholo.Mqtt.Topics.BinderFactoryBuilder
{
    [PublicAPI]
    public class TopicBinderFactoryBuilder
    {
        public static ITopicBinderFactoryBuilder<TTarget> Create<TTarget>()
            where TTarget : class, new()
        {
            return new TopicBinderFactoryBuilder<TTarget>(() => new TTarget());
        }

        public static ITopicBinderFactoryBuilder<TTarget> Create<TTarget>(Func<TTarget> targetFactory)
            where TTarget : class
        {
            return new TopicBinderFactoryBuilder<TTarget>(targetFactory);
        }

        public static ITopicBinderFactoryBuilder<TTarget> CreateDefault<TTarget>()
            where TTarget : class, new()
        {
            return new TopicBinderFactoryBuilder<TTarget>(() => new TTarget()).WithAutoProperties();
        }

        public static ITopicBinderFactoryBuilder<TTarget> CreateDefault<TTarget>(Func<TTarget> targetFactory)
            where TTarget : class
        {
            return new TopicBinderFactoryBuilder<TTarget>(targetFactory).WithAutoProperties();
        }
    }

    [PublicAPI]
    public class TopicBinderFactoryBuilder<TModel> : TopicBinderFactoryBuilder, ITopicBinderFactoryBuilder<TModel>
        where TModel : class
    {
        private Func<TModel> TargetFactory { get; }
        private IDictionary<string, Action<TModel, string>> PropertySetters { get; } = new Dictionary<string, Action<TModel, string>>();
        private ISet<string> UnconfiguredProperties { get; } = new HashSet<string>();

        private IDictionary<string, Func<string, object>> PropertyTypeConverters { get; } = new Dictionary<string, Func<string, object>>();

        private IDictionary<Type, Func<string, object>> TypeConverters { get; } = new Dictionary<Type, Func<string, object>>
        {
            [typeof(bool)] = str => bool.Parse(str),
            [typeof(char)] = str => char.Parse(str),
            [typeof(decimal)] = str => decimal.Parse(str),
            [typeof(double)] = str => double.Parse(str),
            [typeof(float)] = str => float.Parse(str),
            [typeof(int)] = str => int.Parse(str),
            [typeof(uint)] = str => uint.Parse(str),
            [typeof(long)] = str => long.Parse(str),
            [typeof(ulong)] = str => ulong.Parse(str),
            [typeof(short)] = str => short.Parse(str),
            [typeof(ushort)] = str => ushort.Parse(str),
            [typeof(Guid)] = str => Guid.Parse(str),
            [typeof(bool?)] = str => !string.IsNullOrEmpty(str) ? bool.Parse(str) : (bool?)null,
            [typeof(char?)] = str => !string.IsNullOrEmpty(str) ? char.Parse(str) : (char?)null,
            [typeof(decimal?)] = str => !string.IsNullOrEmpty(str) ? decimal.Parse(str) : (decimal?)null,
            [typeof(double?)] = str => !string.IsNullOrEmpty(str) ? double.Parse(str) : (double?)null,
            [typeof(float?)] = str => !string.IsNullOrEmpty(str) ? float.Parse(str) : (float?)null,
            [typeof(int?)] = str => !string.IsNullOrEmpty(str) ? int.Parse(str) : (int?)null,
            [typeof(uint?)] = str => !string.IsNullOrEmpty(str) ? uint.Parse(str) : (uint?)null,
            [typeof(long?)] = str => !string.IsNullOrEmpty(str) ? long.Parse(str) : (long?)null,
            [typeof(ulong?)] = str => !string.IsNullOrEmpty(str) ? ulong.Parse(str) : (ulong?)null,
            [typeof(short?)] = str => !string.IsNullOrEmpty(str) ? short.Parse(str) : (short?)null,
            [typeof(ushort?)] = str => !string.IsNullOrEmpty(str) ? ushort.Parse(str) : (ushort?)null,
            [typeof(Guid?)] = str => !string.IsNullOrEmpty(str) ? Guid.Parse(str) : (Guid?)null,
            [typeof(string)] = str => str
        };

        public TopicBinderFactoryBuilder(Func<TModel> targetFactory)
        {
            TargetFactory = targetFactory;
        }

        public ITopicBinderFactoryBuilder<TModel> WithTypeConverter<TProperty>(Func<string, TProperty> propertyConverter)
        {
            TypeConverters[typeof(TProperty)] = s => propertyConverter.Invoke(s) as object;
            return this;
        }

        public ITopicBinderFactoryBuilder<TModel> WithTypeConverterFor<TProperty>(Expression<Func<TModel, TProperty>> propertySelector, Func<string, TProperty> propertyConverter)
        {
            var memberExpression = (MemberExpression)propertySelector.Body;
            var property = (PropertyInfo)memberExpression.Member;

            PropertyTypeConverters[property.Name] = s => propertyConverter.Invoke(s) as object;
            return this;
        }

        public ITopicBinderFactoryBuilder<TModel> WithAutoProperties()
        {
            var propertySetters = typeof(TModel)
                .GetProperties()
                .Select(x => (name: x.Name, propertyType: x.PropertyType, setMethod: x.GetSetMethod()))
                .Where(x => x.setMethod != null)
                .Where(x => x.setMethod.IsPublic)
                .ToArray();

            foreach (var property in propertySetters)
            {
                if (PropertyTypeConverters.TryGetValue(property.name, out var propertyTypeConverter))
                {
                    PropertySetters[property.name] = (mod, str) => property.setMethod.Invoke(mod, new[] { propertyTypeConverter.Invoke(str) });
                }
                else if (TypeConverters.TryGetValue(property.propertyType, out var typeConverter))
                {
                    PropertySetters[property.name] = (mod, str) => property.setMethod.Invoke(mod, new[] { typeConverter.Invoke(str) });
                }
                else
                {
                    UnconfiguredProperties.Add(property.name);
                }
            }

            return this;
        }

        public ITopicBinderFactoryBuilder<TModel> WithProperty<TProperty>(Expression<Func<TModel, TProperty>> propertySelector, Func<string, TProperty> typeConverter = null)
        {
            var memberExpression = (MemberExpression)propertySelector.Body;
            var property = (PropertyInfo)memberExpression.Member;

            if (PropertySetters.ContainsKey(property.Name))
            {
                throw new ArgumentException($"The property {property.Name} is already mapped", nameof(propertySelector));
            }

            var setMethod = property.GetSetMethod();
            if (setMethod == null)
            {
                throw new ArgumentException($"The property {property.Name} doesn't have a setter", nameof(propertySelector));
            }

            var parameterT = Expression.Parameter(typeof(TModel), "x");
            var parameterTProperty = Expression.Parameter(typeof(TProperty), "y");

            var newExpression =
                Expression.Lambda<Action<TModel, TProperty>>(
                    Expression.Call(parameterT, setMethod, parameterTProperty),
                    parameterT,
                    parameterTProperty
                );

            var propertySetter = newExpression.Compile();

            if (typeConverter != null)
            {
                PropertySetters[property.Name] = (mod, str) => propertySetter.Invoke(mod, typeConverter(str));
            }

            if (PropertyTypeConverters.TryGetValue(property.Name, out var propertyTypeConverter))
            {
                PropertySetters[property.Name] = (mod, str) => propertySetter.Invoke(mod, (TProperty)propertyTypeConverter.Invoke(str));
            }
            else if (TypeConverters.TryGetValue(property.PropertyType, out var globalTypeConverter))
            {
                PropertySetters[property.Name] = (mod, str) => propertySetter.Invoke(mod, (TProperty)globalTypeConverter.Invoke(str));
            }

            UnconfiguredProperties.Remove(property.Name);

            return this;
        }

        public ITopicBinderFactory<TModel> BuildFactory()
        {
            return new TopicBinderFactory<TModel>(TargetFactory, PropertySetters, UnconfiguredProperties);
        }
    }
}
