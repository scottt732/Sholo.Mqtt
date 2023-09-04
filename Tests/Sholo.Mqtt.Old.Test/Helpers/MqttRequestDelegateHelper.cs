using System;
using System.Threading.Tasks;

namespace Sholo.Mqtt.Old.Test.Helpers
{
    public static class MqttRequestDelegateHelper
    {
        public static MqttRequestDelegate HandlesRequest() => Create(_ => true);
        public static MqttRequestDelegate<TTopicParameters> HandlesRequest<TTopicParameters>()
            where TTopicParameters : class, new()
            => Create<TTopicParameters>(_ => true);

        public static MqttRequestDelegate DoesNotHandleRequest() => Create(_ => false);
        public static MqttRequestDelegate<TTopicParameters> DoesNotHandleRequest<TTopicParameters>()
            where TTopicParameters : class, new()
            => Create<TTopicParameters>(_ => false);

        public static MqttRequestDelegate Create(Func<IMqttRequestContext, bool> lambda) => ctx => Task.FromResult(lambda.Invoke(ctx));
        public static MqttRequestDelegate<TTopicParameters> Create<TTopicParameters>(Func<IMqttRequestContext<TTopicParameters>, bool> lambda)
            where TTopicParameters : class, new()
            => ctx => Task.FromResult(lambda.Invoke(ctx));

        public static MqttRequestDelegate CreateAsync(Func<IMqttRequestContext, Task<bool>> lambda) => lambda.Invoke;
        public static MqttRequestDelegate<TTopicParameters> CreateAsync<TTopicParameters>(Func<IMqttRequestContext<TTopicParameters>, Task<bool>> lambda)
            where TTopicParameters : class, new()
            => lambda.Invoke;

        public static MqttRequestDelegate Throws(Exception exception) => Create(_ => throw exception);
        public static MqttRequestDelegate<TTopicParameters> Throws<TTopicParameters>(Exception exception)
            where TTopicParameters : class, new()
            => Create<TTopicParameters>(_ => throw exception);

        public static MqttRequestDelegate Throws(Func<Exception> exceptionFactory) => Create(_ => throw exceptionFactory.Invoke());
        public static MqttRequestDelegate<TTopicParameters> Throws<TTopicParameters>(Func<Exception> exceptionFactory)
            where TTopicParameters : class, new()
            => Create<TTopicParameters>(_ => throw exceptionFactory.Invoke());
    }
}
