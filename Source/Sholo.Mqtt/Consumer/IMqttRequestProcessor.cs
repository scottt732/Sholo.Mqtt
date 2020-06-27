using System.Threading.Tasks;

namespace Sholo.Mqtt.Consumer
{
    public interface IMqttRequestProcessor
    {
        Task<bool> ProcessRequest(MqttRequestContext context);
    }

    public interface IMqttRequestProcessor<TTopicParameters>
        where TTopicParameters : class, new()
    {
        Task<bool> ProcessRequest(MqttRequestContext<TTopicParameters> context);
    }
}
