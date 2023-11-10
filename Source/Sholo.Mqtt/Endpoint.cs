using System.Reflection;
using System.Threading;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt;

[PublicAPI]
public class Endpoint
{
    public MethodInfo Action { get; }
    public IMqttTopicFilter TopicFilter { get; }
    public MqttRequestDelegate RequestDelegate { get; }

    public Endpoint(
        MethodInfo action,
        IMqttTopicFilter topicPatternFilter,
        MqttRequestDelegate requestDelegate)
    {
        Action = action;
        TopicFilter = topicPatternFilter;
        RequestDelegate = requestDelegate;
    }

    public bool IsMatch(IMqttRequestContext context)
    {
        if (TopicFilter.IsMatch(context, out var topicArguments) && TopicFilter.QualityOfServiceLevel == context.QualityOfServiceLevel)
        {
            var actionParameters = Action.GetParameters();
            var requiredArguments = actionParameters.Length;

            foreach (var actionParameter in actionParameters)
            {
                var parameterName = actionParameter.Name!;

                if (topicArguments?.TryGetValue(parameterName, out _) ?? false)
                {
                    requiredArguments--;
                }
                else if (actionParameter.ParameterType == typeof(CancellationToken))
                {
                    requiredArguments--;
                }
                else if (context.ServiceProvider.GetService(actionParameter.ParameterType) != null)
                {
                    requiredArguments--;
                }

                // TODO: Better handling for when the request has a model (was break instead of continue above, requiredArguments == 0)
                if (requiredArguments <= 1)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
