using System.Text.Json;

namespace Sholo.Mqtt.TypeConverters;

[PublicAPI]
public class JsonTypeConverterOptions
{
    public JsonReaderOptions JsonReaderOptions { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();
}
