using JetBrains.Annotations;

namespace Sholo.Mqtt.Topic
{
    [PublicAPI]
    public interface ITopicBinderFactory<out TModel>
    {
        ITopicBinder<TModel> CreateBinder(string pattern);
    }
}
