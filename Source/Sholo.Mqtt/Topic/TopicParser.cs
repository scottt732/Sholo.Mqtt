using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Sholo.Mqtt.Topic
{
    [PublicAPI]
    public class TopicParser
    {
        public Regex ToMatchingRegex(string pattern)
        {
            var topicParts = pattern.Split('/');
            var regBuilder = new StringBuilder();

            var haveMultiLevelWildcard = false;
            for (var i = 0; i < topicParts.Length; i++)
            {
                var topicPart = topicParts[i];
                if (topicPart.Equals("+", StringComparison.Ordinal))
                {
                    regBuilder.Append("(?<Wildcard>[^/]+)");
                }
                else if (topicPart.Equals("#", StringComparison.Ordinal))
                {
                    if (i != topicParts.Length - 1)
                    {
                        throw new ArgumentException("Multi-level wildcards can only appear at the end of a topic pattern");
                    }

                    if (haveMultiLevelWildcard)
                    {
                        throw new ArgumentException("Only one multi-level wildcard can appear within a topic pattern");
                    }

                    regBuilder.Append("(?<MultiLevelWildcard>([^/]+/)*([^/]+))");
                    haveMultiLevelWildcard = true;
                }
                else
                {
                    regBuilder.Append(topicPart);
                }

                regBuilder.Append('/');
            }

            if (regBuilder.Length > 0)
            {
                regBuilder.Length -= 1;
            }

            return new Regex(regBuilder.ToString());
        }
    }
}
