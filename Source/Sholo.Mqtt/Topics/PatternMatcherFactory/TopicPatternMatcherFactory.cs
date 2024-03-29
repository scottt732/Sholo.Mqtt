using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sholo.Mqtt.Topics.PatternMatcher;

namespace Sholo.Mqtt.Topics.PatternMatcherFactory;

// TODO: It may be possible to enumerate all of the patterns at compile time, in which case a code generator can precompute/compile these and a provider can be used to retrieve them without the runtime overhead.
// TODO: In the meantime, I opted to compile the regular expressions created since the ratio of instance usage to instance creation is likely high enough to justify the setup cost.
internal class TopicPatternMatcherFactory : ITopicPatternMatcherFactory
{
    public ITopicPatternMatcher CreateTopicPatternMatcher(string topicPattern) // TODO: , bool caseSensitive
    {
        if (topicPattern == null) throw new ArgumentNullException(nameof(topicPattern), $"{nameof(topicPattern)} is required.");
        if (string.IsNullOrEmpty(topicPattern)) throw new ArgumentException($"{nameof(topicPattern)} must be non-empty.", nameof(topicPattern));

        if (!topicPattern.Any(c => c is '+' or '#'))
        {
            return new SimpleTopicPatternMatcher(topicPattern);
        }

        var topicParts = topicPattern.Split('/');
        var regBuilder = new StringBuilder();
        string? mutliLevelWildcardVariableName = null;

        var stringComparer = StringComparer.Ordinal;

        var topicParameterNames = new HashSet<string>(stringComparer);
        for (var i = 0; i < topicParts.Length; i++)
        {
            var topicPart = topicParts[i];
            if (topicPart.StartsWith('+'))
            {
                var variableName = topicPart[1..];
                topicParameterNames.Add(variableName);

                if (variableName.Length == 0)
                {
                    throw new ArgumentException(
                        "The topic pattern specified must contain a variable name after + or # characters (e.g., Test/+Username/#Options). " +
                        "This will be translated to an MQTT subscription (Test/+/#) and the values will be used to expose named properties",
                        nameof(topicPattern)
                    );
                }

                regBuilder.Append("(?<" + variableName + ">[^/]+)");
            }
            else if (topicPart.StartsWith('#'))
            {
                if (i != topicParts.Length - 1)
                {
                    throw new ArgumentException("Multi-level wildcards can only appear at the end of a topic pattern.", nameof(topicPattern));
                }

                mutliLevelWildcardVariableName = topicPart[1..];
                topicParameterNames.Add(mutliLevelWildcardVariableName);

                if (mutliLevelWildcardVariableName.Length == 0)
                {
                    throw new ArgumentException(
                        "The topic pattern specified must contain a variable name after + or # characters (e.g., Test/+Username/#Options). " +
                        "This will be translated to an MQTT subscription (Test/+/#) and the values will be used to expose named properties",
                        nameof(topicPattern)
                    );
                }

                regBuilder.Append("(?<" + mutliLevelWildcardVariableName + ">([^/]+/)*([^/]+))");
            }
            else
            {
                regBuilder.Append(topicPart);
            }

            regBuilder.Append('/');
        }

        if (regBuilder is { Length: > 0 })
        {
            // Remove trailing '/'
            regBuilder.Length -= 1;
        }

        regBuilder.Append('$');

        var regexOptions = RegexOptions.Compiled;

        /*
        if (!caseSensitive)
        {
            regexOptions |= RegexOptions.IgnoreCase;
        }
        */

        var regex = new Regex(regBuilder.ToString(), regexOptions);

        return new ComplexTopicPatternMatcher(topicPattern, regex, topicParameterNames, mutliLevelWildcardVariableName);
    }
}
