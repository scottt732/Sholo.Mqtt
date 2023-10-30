using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Sholo.Mqtt.TypeConverters.NewtonsoftJson;

public class NewtonsoftJsonTypeConverter : IMqttRequestPayloadTypeConverter
{
    private ILogger Logger { get; }
    private Encoding Encoding { get; }
    private JsonSerializer Serializer { get; }

    public NewtonsoftJsonTypeConverter(ILogger<NewtonsoftJsonTypeConverter> logger, IOptions<NewtonsoftJsonTypeConverterOptions> options)
    {
        Logger = logger;
        Encoding = options.Value.Encoding;
        Serializer = JsonSerializer.Create(options.Value.SerializerSettings);
    }

    public bool TryConvertPayload(ArraySegment<byte> payloadData, Type targetType, out object result) => TryConvert(payloadData, targetType, out result);

    private bool TryConvert(ArraySegment<byte> data, Type targetType, out object result)
    {
        string sourceData = null;
        try
        {
            sourceData = Encoding.GetString(data);
            var textReader = new StringReader(sourceData);

            result = Serializer.Deserialize(textReader, targetType);

            return true;
        }
        catch (ArgumentException ae)
        {
            Logger.LogWarning(ae, "Failed to read payload as JSON with ArgumentException for type {TargetType}: {SourceData}", targetType, sourceData);
            result = null;
            return false;
        }
        catch (JsonSerializationException jse)
        {
            Logger.LogWarning(jse, "Failed to read payload as JSON with JsonSerializationException for type {TargetType}: {SourceData}", targetType, sourceData);
            result = null;
            return false;
        }
        catch (JsonReaderException jre)
        {
            Logger.LogWarning(jre, "Failed to read payload as JSON with JsonReaderException for type {TargetType}: {SourceData}", targetType, sourceData);
            result = null;
            return false;
        }
    }
}
