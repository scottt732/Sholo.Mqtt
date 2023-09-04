using System.Reflection;
using System.Threading;
using Sholo.Mqtt.Topics.PatternFilter;

namespace Sholo.Mqtt
{
    public class Endpoint
    {
        public MethodInfo Action { get; }
        public IMqttTopicPatternFilter TopicPatternFilter { get; }
        public MqttRequestDelegate RequestDelegate { get; }

        public Endpoint(
            MethodInfo action,
            IMqttTopicPatternFilter topicPatternFilter,
            MqttRequestDelegate requestDelegate)
        {
            Action = action;
            TopicPatternFilter = topicPatternFilter;
            RequestDelegate = requestDelegate;
        }

        public bool IsMatch(MqttRequestContext context)
        {
            if (TopicPatternFilter.IsMatch(context.Topic, out var topicParameters)
                && TopicPatternFilter.QualityOfServiceLevel == context.QualityOfServiceLevel)
            {
                var actionParameters = Action.GetParameters();
                var requiredArguments = actionParameters.Length;

                foreach (var actionParameter in actionParameters)
                {
                    var parameterName = actionParameter.Name;

                    if (topicParameters.TryGetValue(parameterName, out _))
                    {
                        requiredArguments--;
                    }
                    else if (actionParameter.ParameterType == typeof(CancellationToken))
                    {
                        requiredArguments--;
                    }
                    else
                    {
                        if (context.ServiceProvider.GetService(actionParameter.ParameterType) != null)
                        {
                            requiredArguments--;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                // TODO: Better handling for when the request has a model (was break instead of continue above, requiredArguments == 0)
                if (requiredArguments <= 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
