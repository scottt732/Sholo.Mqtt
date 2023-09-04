using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sholo.Mqtt.TypeConverters.Payload.NewtonsoftJson;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Sholo.Mqtt.TypeConverters.NewtonsoftJson;

public class NewtonsoftJsonTypeConverter : IMqttRequestPayloadTypeConverter
{
    private Encoding Encoding { get; }
    private JsonSerializer Serializer { get; }

    public NewtonsoftJsonTypeConverter(IOptions<NewtonsoftJsonTypeConverterOptions> options)
    {
        Encoding = options.Value.Encoding;
        Serializer = JsonSerializer.Create(options.Value.SerializerSettings);
    }

    public bool TryConvertPayload(byte[] payloadData, Type targetType, out object result) => TryConvert(payloadData, targetType, out result);

    private bool TryConvert(byte[] data, Type targetType, out object result)
    {
        try
        {
            var sourceData = Encoding.GetString(data);
            var textReader = new StringReader(sourceData);

            result = Serializer.Deserialize(textReader, targetType);

            return true;
        }
        catch (ArgumentException)
        {
            result = null;
            return false;
        }
        catch (JsonSerializationException)
        {
            result = null;
            return false;
        }
        catch (JsonReaderException)
        {
            result = null;
            return false;
        }
    }
}
