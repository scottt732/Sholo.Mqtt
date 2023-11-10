using System.Collections.Generic;

namespace Sholo.Mqtt.Topics.PatternMatcher;

[PublicAPI]
public interface ITopicPatternMatcher
{
    /// <summary>
    ///     Gets the MQTT protocol compliant topic to subscribe to. This does not include library-specific variable
    ///     names as <see cref="TopicPattern"/> does.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Gets the topic mask to subscribe to. This includes library-specific variable names that will be replaced at
    ///     runtime with the values from incoming messages. For example, A <see cref="TopicPattern"/> value of
    ///     <em>test/+topic_part/parts/#topic_parts</em> corresponds to a <see cref="Topic"/> value of <em>test/+/parts/#</em>.
    ///     The former is used for binding from consumed messages whereas the latter is MQTT-compliant for establishing a subscription.
    /// </summary>
    string TopicPattern { get; }

    /// <summary>
    ///     Gets the names of the topic parameters in the <see cref="TopicPattern" />. For example, in
    ///     <em>test/+topic_part/parts/#topic_parts</em>, this would contain ['topic_part', 'topic_parts']. These would be
    ///     stripped from the topic in the MQTT subscription (subscribing to <em>test/+/parts/#</em>).
    /// </summary>
    IReadOnlySet<string> TopicParameterNames { get; }

    /// <summary>
    ///     Gets the name of the multi-level wildcard variable, if any, in the <see cref="TopicPattern" />. For example, in
    ///     <em>test/+topic_part/parts/#topic_parts</em> this would be <em>topic_parts</em> whereas in
    ///     <em>test/+topic_part/parts</em> it would be <em>null</em>. It is the value after the # symbol, which by MQTT
    ///     protocol must be in the last fragment of the topic.
    /// </summary>
    string? MutliLevelWildcardParameterName { get; }

    /// <summary>
    ///     Indicates whether the topic passed matches the topic pattern. If so, the arguments are returned in
    ///     the form of a dictionary whose keys are among the values in the topic pattern and whose values
    ///     are extracted from the <paramref name="topic" /> supplied.
    /// </summary>
    /// <param name="topic">The topic to check against the topic pattern</param>
    /// <param name="topicArguments">
    ///     A dictionary of keys (extracted from the topic pattern) &amp; values
    ///     extracted from the <paramref name="topic" />
    /// </param>
    /// <returns>A boolean indicating whether or not the topic supplied matches the filter</returns>
    bool IsTopicMatch(string topic, out IReadOnlyDictionary<string, string[]>? topicArguments);
}
