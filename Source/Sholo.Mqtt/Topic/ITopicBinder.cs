// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.Mqtt.Topic
{
    public interface ITopicBinder<out TModel>
    {
        string MqttPattern { get; }
        bool IsMatch(string topic);
        TModel Bind(string topic);
    }
}
