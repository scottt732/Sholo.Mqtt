using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Sholo.Mqtt.ModelBinding;

public interface IMqttModelBinder
{
    /// <summary>
    ///     Attempts to bind the request details to the matched action method's arguments. The results
    ///     of the binding operation are stored in the <see cref="IMqttRequestContext"/>'s
    ///     <see cref="IMqttRequestContext.ModelBindingResult" /> property."/>
    /// </summary>
    /// <param name="modelBindingContext">
    ///     The <see cref="IMqttModelBindingContext"/> associated with the endpoint or action.
    /// </param>
    /// <param name="requestContext">
    ///     An <see cref="IMqttRequestContext"/> associated with the incoming request
    /// </param>
    /// <param name="topicArguments">
    ///     An <see cref="IReadOnlyDictionary{String,StringValues}"/> containing the arguments
    ///     extracted from the incoming <paramref name="requestContext" /> using the
    ///     <paramref name="modelBindingContext"/>'s <see cref="IMqttModelBindingContext.TopicFilter"/>
    ///     topic filter during the route matching operation.
    /// </param>
    void TryPerformModelBinding(
        IMqttModelBindingContext modelBindingContext,
        IMqttRequestContext requestContext,
        IReadOnlyDictionary<string, StringValues> topicArguments
    );
}
