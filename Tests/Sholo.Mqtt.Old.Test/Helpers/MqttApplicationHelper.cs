using JetBrains.Annotations;

namespace Sholo.Mqtt.Old.Test.Helpers
{
    [PublicAPI]
    public static class MqttApplicationHelper
    {
        /*
        public static IMqttApplication CreateHandlesAllRequests()
            => new MqttApplicationBuilder()
                .UseDefault()
                .Build();

        public static IMqttApplication CreateDoesNotHandleRequests()
            => new MqttApplicationBuilder()
                .Build();

        public static IMqttApplication CreateThrowsDuringRequestProcessing(Exception exception)
            => new MqttApplicationBuilder()
                .Use(MqttRequestDelegateHelper.Throws(exception))
                .Build();

        public static IMqttApplication CreateThrowsDuringRequestProcessing(Func<Exception> exceptionFactory)
            => new MqttApplicationBuilder()
                .Use(MqttRequestDelegateHelper.Throws(exceptionFactory))
                .Build();
        */
    }
}
