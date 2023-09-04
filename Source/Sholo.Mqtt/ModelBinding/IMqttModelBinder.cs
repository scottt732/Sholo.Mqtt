using System;

namespace Sholo.Mqtt.ModelBinding;

public interface IMqttModelBinder
{
    bool CanBind(Type targetType);
}

public interface IMqttModelBinder<TSource> : IMqttModelBinder
{
}

public interface IMqttModelBinder<TSource, TTarget> : IMqttModelBinder<TSource>
{
    bool TryGetValue(TSource source, out TTarget target);
}
