using System.Threading.Tasks;

namespace Sholo.Mqtt.Old.RequestProcessor
{
    public interface IMqttRequestProcessor
    {
        Task<bool> ProcessRequest(IMqttRequestContext context);
    }

    public interface IMqttRequestProcessor<in TTopicParameters>
        where TTopicParameters : class
    {
        Task<bool> ProcessRequest(IMqttRequestContext<TTopicParameters> context);
    }
}
