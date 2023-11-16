using System.Collections.Generic;
using System.Reflection;
using Sholo.Mqtt.ModelBinding.Validation;
using Sholo.Mqtt.ModelBinding.ValueProviders;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

public class MqttModelBindingContext : IMqttModelBindingContext
{
    public MqttBindingSource? BindingSource { get; }
    public IMqttRequestContext RequestContext { get; }
    public IMqttTopicFilter TopicFilter { get; }
    public IReadOnlyDictionary<string, string[]> TopicArguments { get; }
    public MethodInfo Action { get; }
    public IDictionary<ParameterInfo, object?> ActionArguments { get; }
    public IDictionary<ParameterInfo, IMqttValueProvider> ParameterValueProviders { get; }
    public MqttValidationStateDictionary MqttValidationState { get; }
    public MqttModelBindingResult Result { get; set; }

    /*
    public MethodInfo Action { get; }
    public string TopicPattern { get; }
    public IReadOnlyDictionary<string, string[]> TopicArguments { get; }
    public IMqttRequestContext Request { get; }
    public ILogger? Logger { get; }
    public IMqttParameterTypeConverter[] ParameterTypeConverters => LazyParameterTypeConverters.Value;
    public IMqttCorrelationDataTypeConverter CorrelationDataTypeConverter => LazyCorrelationDataTypeConverter.Value;
    public IMqttPayloadTypeConverter PayloadTypeConverter => LazyPayloadTypeConverter.Value;

    private Lazy<IMqttParameterTypeConverter[]> LazyParameterTypeConverters { get; }
    private Lazy<IMqttCorrelationDataTypeConverter> LazyCorrelationDataTypeConverter { get; }
    private Lazy<IMqttPayloadTypeConverter> LazyPayloadTypeConverter { get; }

    public MqttModelBindingContext(
        MethodInfo action,
        string topicPattern,
        IMqttRequestContext request,
        IReadOnlyDictionary<string, string[]> topicArguments,
        ILogger? logger
    )
    {
        Action = action;
        TopicPattern = topicPattern;
        Request = request;
        Logger = logger;
        TopicArguments = topicArguments;

        LazyParameterTypeConverters = new Lazy<IMqttParameterTypeConverter[]>(RetrieveParameterTypeConverters);
        LazyCorrelationDataTypeConverter = new Lazy<IMqttCorrelationDataTypeConverter>(RetrieveCorrelationDataTypeConverter);
        LazyPayloadTypeConverter = new Lazy<IMqttPayloadTypeConverter>(RetrievePayloadTypeConverter);
    }

    public bool TryConvertParameter(string? input, IMqttParameterTypeConverter? explicitParameterTypeConverter, ParameterInfo actionParameter, Type targetType, out object? result)
    {
        if (input == null)
        {
            if (actionParameter.ParameterType.IsClass)
            {
                result = default;
                return true;
            }

            if (actionParameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(actionParameter.ParameterType) != null)
            {
                result = null;
                return true;
            }

            result = null;
            return false;
        }

        if (explicitParameterTypeConverter != null)
        {
            if (explicitParameterTypeConverter.TryConvertParameter(input, targetType, out result))
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException(
                    $"The converter {explicitParameterTypeConverter.GetType().Name} cannot convert parameters of type {actionParameter.ParameterType.Name}");
            }
        }

        foreach (var converter in ParameterTypeConverters)
        {
            if (converter.TryConvertParameter(input, targetType, out result))
            {
                return true;
            }
        }

        if (DefaultTypeConverters.TryConvert(input, actionParameter.ParameterType, out result))
        {
            return true;
        }

        result = default;
        return false;
    }

    private IMqttParameterTypeConverter[] RetrieveParameterTypeConverters() =>
        Request.ServiceProvider.GetService<IEnumerable<IMqttParameterTypeConverter>>()
            ?.Reverse()
            .ToArray()
        ?? new IMqttParameterTypeConverter[] { DefaultTypeConverters.Instance };

    private IMqttCorrelationDataTypeConverter RetrieveCorrelationDataTypeConverter() =>
        Request.ServiceProvider.GetService<IMqttCorrelationDataTypeConverter>()
            ?? DefaultTypeConverters.Instance;

    private IMqttPayloadTypeConverter RetrievePayloadTypeConverter() =>
        Request.ServiceProvider.GetService<IMqttPayloadTypeConverter>()
            ?? DefaultTypeConverters.Instance;
    */
}
