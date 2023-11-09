using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sholo.Mqtt.TypeConverters;

public class JsonTypeConverter : IMqttRequestPayloadTypeConverter
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

    private bool TryConvert(ArraySegment<byte> data, Type targetType, out object? result)
    {
        try
        {
            var jsonReader = new Utf8JsonReader(data, Options.Value.JsonReaderOptions);
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
