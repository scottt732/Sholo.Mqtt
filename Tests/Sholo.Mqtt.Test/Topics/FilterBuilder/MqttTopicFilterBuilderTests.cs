using System;
using Sholo.Mqtt.Topics.Filter;
using Sholo.Mqtt.Topics.FilterBuilder;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.FilterBuilder;

public class MqttTopicFilterBuilderTests
{
    [Fact]
    public void Build_WhenTopicIsNull_ThrowsArgumentNullException()
    {
        var ane = Assert.Throws<ArgumentNullException>(() => new MqttTopicFilterBuilder().Build());

        Assert.StartsWith($"The {nameof(IMqttTopicFilter.Topic)} cannot be null", ane.Message, StringComparison.Ordinal);
        Assert.Equal($"{nameof(IMqttTopicFilter.Topic)}", ane.ParamName);
    }

    [Fact]
    public void Build_WhenTopicIsEmpty_ThrowsArgumentNullException()
    {
        var ae = Assert.Throws<ArgumentException>(() => new MqttTopicFilterBuilder().WithTopicPattern(string.Empty).Build());

        Assert.StartsWith($"The {nameof(IMqttTopicFilter.Topic)} must be non-empty", ae.Message, StringComparison.Ordinal);
        Assert.Equal($"{nameof(IMqttTopicFilter.Topic)}", ae.ParamName);
    }
}
