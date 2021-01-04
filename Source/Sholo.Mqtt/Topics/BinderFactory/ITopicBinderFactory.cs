using JetBrains.Annotations;
using Sholo.Mqtt.Topics.Binder;

namespace Sholo.Mqtt.Topics.BinderFactory
{
    [PublicAPI]
    public interface ITopicBinderFactory<out TModel>
    {
        ITopicBinder<TModel> CreateBinder(string pattern);
    }
}
