using System.Text;
using Newtonsoft.Json;

namespace Sholo.Mqtt.TypeConverters.NewtonsoftJson;

public class NewtonsoftJsonTypeConverterOptions
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings();
}
