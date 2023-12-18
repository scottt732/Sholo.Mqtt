using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.Internal;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;

[PublicAPI]
public abstract class BaseFromMqttConverterAttribute<TMqttTypeConverterInterface, TMqttTypeConverter> : Attribute, IFromMqttConverter
    where TMqttTypeConverterInterface : class, IMqttTypeConverter
    where TMqttTypeConverter : class, TMqttTypeConverterInterface
{
    public MqttBindingSource BindingSource { get; }
    public string? ServiceKey { get; }

    protected BaseFromMqttConverterAttribute(MqttBindingSource bindingSource, string? serviceKey = null)
    {
        BindingSource = bindingSource;
        ServiceKey = serviceKey;
    }

    public bool TryBind(IMqttRequestContext requestContext, ParameterState parameterState, out object? result)
    {
        if (TryGetTypeConverter(requestContext.ServiceProvider, ServiceKey, out var typeConverterInstance)
            && TryConvert(requestContext, parameterState, typeConverterInstance, out result))
        {
            return true;
        }

        result = null;
        return false;
    }

    protected abstract bool TryConvert(
        IMqttRequestContext requestContext,
        ParameterState parameterState,
        TMqttTypeConverterInterface typeConverter,
        out object? result
    );

    protected virtual bool TryGetTypeConverter(IServiceProvider serviceProvider, string? serviceKey, [MaybeNullWhen(false)] out TMqttTypeConverterInterface typeConverter)
    {
        typeConverter = !string.IsNullOrEmpty(serviceKey) ? serviceProvider.GetKeyedService<TMqttTypeConverterInterface>(serviceKey) : null;
        if (typeConverter != null) return true;

        typeConverter = serviceProvider.GetService<TMqttTypeConverterInterface>();
        if (typeConverter != null) return true;

        var typeActivatorCache = serviceProvider.GetService<ITypeActivatorCache>();
        typeConverter = typeActivatorCache?.CreateInstance<TMqttTypeConverterInterface>(serviceProvider, typeof(TMqttTypeConverter));
        if (typeConverter != null) return true;

        typeConverter = null;
        return false;
    }
}
