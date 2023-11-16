using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Json;

public class JsonTypeConverter : IMqttPayloadTypeConverter, IMqttCorrelationDataTypeConverter
{
    private IOptions<JsonTypeConverterOptions> Options { get; }
    private ILogger Logger { get; }

    public JsonTypeConverter(
        IOptions<JsonTypeConverterOptions> options,
        ILogger<JsonTypeConverter> logger
    )
    {
        Options = options;
        Logger = logger;
    }

    public bool TryConvertPayload(ArraySegment<byte> payloadData, Type targetType, out object? result)
        => TryConvert(payloadData, targetType, out result);

    public bool TryConvert(byte[]? input, Type targetType, out object? result)
    {
        if (input == null)
        {
            result = null;
            return false;
        }

        return TryConvert(new ArraySegment<byte>(input), targetType, out result);
    }

    [ExcludeFromCodeCoverage]
    public bool TryConvert(ArraySegment<byte> input, Type targetType, out object? result)
    {
        try
        {
            var jsonReader = new Utf8JsonReader(input, Options.Value.JsonReaderOptions);
            result = JsonSerializer.Deserialize(ref jsonReader, targetType, Options.Value.JsonSerializerOptions);
            return true;
        }
        catch (Exception exc)
        {
            Logger.LogWarning(exc, "Failed to read payload as JSON with Exception for type {TargetType}", targetType);
            result = null;
            return false;
        }
    }
}
