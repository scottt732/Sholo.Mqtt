namespace Sholo.Mqtt.ModelBinding.Context;

/*
[PublicAPI]
public interface IMqttModelBindingContext
{
    IMqttRequestContext Request { get; }
    MethodInfo Action { get; }
    string TopicPattern { get; }
    ILogger? Logger { get; }

    // Parameters binding
    IReadOnlyDictionary<string, string[]> TopicArguments { get; }
    IMqttParameterTypeConverter[] ParameterTypeConverters { get; }

    // Correlation Data
    IMqttCorrelationDataTypeConverter CorrelationDataTypeConverter { get; }

    // Payload binding
    IMqttPayloadTypeConverter PayloadTypeConverter { get; }

    bool TryConvertParameter(string? input, IMqttParameterTypeConverter? explicitParameterTypeConverter, ParameterInfo actionParameter, Type targetType, out object? result);
}
*/
