namespace Sholo.Mqtt.Test.Topics.PatternFilterBuilder;

public class MqttTopicPatternFilterBuilderTests
{
    /*
    [Theory]
    [CombinatorialData]
    public void WithQualityOfServiceLevel_WhenBuiltWithValidValue_ReflectsDesiredValue(MqttQualityOfServiceLevel qualityOfServiceLevel)
    {
        var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
            .WithTopicPattern("test")
            .WithQualityOfServiceLevel(qualityOfServiceLevel)
            .Build();

        Assert.Equal(qualityOfServiceLevel, mqttTopicPatternFilter.QualityOfServiceLevel);
    }

    [Theory]
    [InlineData("testing/+testing", "testing/+", new[] { "testing" })]
    [InlineData("testing/+testing/123", "testing/+/123", new[] { "testing" })]
    [InlineData("testing/#testing", "testing/#", new[] { "testing" })]
    [InlineData("#all", "#", new[] { "all" })]
    [InlineData("/", "/", new string[0])]
    public void WithTopicPattern_WhenBuiltWithValidValue_ReflectsDesiredValue(string topicPattern, string expectedTopic, string[] topicParameterNames)
    {
        var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
            .WithTopicPattern(topicPattern)
            .Build();

        Assert.Equal(expectedTopic, mqttTopicPatternFilter.Topic);
        Assert.Equal(topicPattern, mqttTopicPatternFilter.TopicPattern);
        Assert.Equal(topicParameterNames, mqttTopicPatternFilter.TopicParameterNames);
    }

    [Theory]
    [InlineData("testing/+")]
    [InlineData("testing/+/123")]
    [InlineData("testing/#")]
    [InlineData("#")]
    public void WithTopicPattern_WhenWildcardVariableNameIsMissing_ThrowsArgumentException(string topicPattern)
    {
        var ae = Assert.Throws<ArgumentException>(() => new MqttTopicPatternFilterBuilder()
            .WithTopicPattern(topicPattern)
            .Build());

        Assert.StartsWith("The topic pattern specified must contain a variable name after", ae.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WithTopicPattern_WhenEmpty_ThrowsArgumentException()
    {
        var ae = Assert.Throws<ArgumentException>(() => new MqttTopicPatternFilterBuilder()
            .WithTopicPattern(string.Empty)
            .Build());

        Assert.StartsWith("TopicPattern must be non-empty.", ae.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("#im/invalid")]
    [InlineData("this/#is/invalid")]
    public void Build_WhenTopicPatternContainsMultiLevelWildcardNotAtEndOfString_ThrowsArgumentException(string invalidTopicPattern)
    {
        var ae = Assert.Throws<ArgumentException>(() => new MqttTopicPatternFilterBuilder().WithTopicPattern(invalidTopicPattern).Build());

        Assert.StartsWith("Multi-level wildcards can only appear at the end of a topic pattern.", ae.Message, StringComparison.Ordinal);
    }

    [Theory]
    [CombinatorialData]
    public void WithNoLocal_WhenBuiltWithValidValue_ReflectsDesiredValue(bool noLocal)
    {
        var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
            .WithTopicPattern("test")
            .WithNoLocal(noLocal)
            .Build();

        Assert.Equal(noLocal, mqttTopicPatternFilter.NoLocal);
    }

    [Theory]
    [CombinatorialData]
    public void WithRetainAsPublished_WhenBuiltWithValidValue_ReflectsDesiredValue(bool retainAsPublished)
    {
        var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
            .WithTopicPattern("test")
            .WithRetainAsPublished(retainAsPublished)
            .Build();

        Assert.Equal(retainAsPublished, mqttTopicPatternFilter.RetainAsPublished);
    }

    [Theory]
    [CombinatorialData]
    public void WithRetainHandling_WhenBuiltWithValidValue_ReflectsDesiredValue(MqttRetainHandling retainHandling)
    {
        var mqttTopicPatternFilter = new MqttTopicPatternFilterBuilder()
            .WithTopicPattern("test")
            .WithRetainHandling(retainHandling)
            .Build();

        Assert.Equal(retainHandling, mqttTopicPatternFilter.RetainHandling);
    }

    [Fact]
    public void Build_WhenTopicPatternIsOmitted_ThrowsArgumentNullException()
    {
        var ane = Assert.Throws<ArgumentNullException>(() => new MqttTopicPatternFilterBuilder().Build());

        Assert.StartsWith("TopicPattern is required.", ane.Message, StringComparison.Ordinal);
    }
    */
}
