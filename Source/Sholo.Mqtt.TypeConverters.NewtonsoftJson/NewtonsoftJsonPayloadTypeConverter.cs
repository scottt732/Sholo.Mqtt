using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Sholo.Mqtt.TypeConverters.NewtonsoftJson;

public class NewtonsoftJsonPayloadTypeConverter : IMqttPayloadTypeConverter
{
    private ILogger Logger { get; }
    private Encoding Encoding { get; }
    private JsonSerializer Serializer { get; }

    public NewtonsoftJsonPayloadTypeConverter(
        IOptions<NewtonsoftJsonTypeConverterOptions> options,
        ILogger<NewtonsoftJsonPayloadTypeConverter> logger
    )
    {
        Logger = logger;
        Encoding = options.Value.Encoding;
        Serializer = JsonSerializer.Create(options.Value.SerializerSettings);
    }

    public bool TryConvertPayload(ArraySegment<byte> payload, Type targetType, out object result) => TryConvert(payload, targetType, out result);

    private bool TryConvert(ArraySegment<byte> data, Type targetType, out object result)
    {
        try
        {
            var textReader = new StringReader(Encoding.GetString(data));

            result = Serializer.Deserialize(textReader, targetType);

            return true;
        }
        catch (ArgumentException ae)
        {
            Logger.LogWarning(ae, "Failed to read payload as JSON with ArgumentException for type {TargetType}", targetType);
            result = null;
            return false;
        }
        catch (JsonSerializationException jse)
        {
            Logger.LogWarning(jse, "Failed to read payload as JSON with JsonSerializationException for type {TargetType}", targetType);
            result = null;
            return false;
        }
        catch (JsonReaderException jre)
        {
            Logger.LogWarning(jre, "Failed to read payload as JSON with JsonReaderException for type {TargetType}", targetType);
            result = null;
            return false;
        }
    }
}
