// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.Mqtt.Topics.Binder
{
    public interface ITopicBinder<out TModel>
    {
        string MqttPattern { get; }
        bool IsMatch(string topic);
        TModel Bind(string topic);
    }
}
