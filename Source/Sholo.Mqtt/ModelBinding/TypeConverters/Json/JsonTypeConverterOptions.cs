using System.Text.Json;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Json;

[PublicAPI]
public class JsonTypeConverterOptions
{
    public JsonReaderOptions JsonReaderOptions { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();
}
