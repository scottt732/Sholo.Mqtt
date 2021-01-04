using System.Threading.Tasks;

namespace Sholo.Mqtt.RequestProcessor
{
    public interface IMqttRequestProcessor
    {
        Task<bool> ProcessRequest(IMqttRequestContext context);
    }

    public interface IMqttRequestProcessor<in TTopicParameters>
        where TTopicParameters : class, new()
    {
        Task<bool> ProcessRequest(IMqttRequestContext<TTopicParameters> context);
    }
}
