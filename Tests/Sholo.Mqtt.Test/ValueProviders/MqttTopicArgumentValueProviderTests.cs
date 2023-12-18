using Moq;
using Sholo.Mqtt.ModelBinding;

namespace Sholo.Mqtt.Test.ValueProviders;

public class MqttTopicArgumentValueProviderTests
{
    private Mock<IMqttModelBindingContext> MockModelBindingContext { get; } = new(MockBehavior.Strict);

    /*
    [Fact]
    public void GetValueSource_WhenTopicHasParameter_ReturnsValue()
    {
        var mqttTopicArgumentValueProvider = new MqttTopicArgumentValueProvider("test");

        var expectedResults = new[] { "result" };

        MockModelBindingContext
            .SetupGet(x => x.TopicArguments)
            .Returns(
                new ReadOnlyDictionary<string, StringValues>(
                    new Dictionary<string, StringValues>
                    {
                        ["test"] = expectedResults
                    }
                )
            );

        var results = mqttTopicArgumentValueProvider.GetValueSource(MockModelBindingContext.Object);

        Assert.NotNull(results);
        Assert.True(results.SequenceEqual(expectedResults));
    }

    [Fact]
    public void GetValueSource_WhenTopicHasParameter_ReturnsNull()
    {
        var mqttTopicArgumentValueProvider = new MqttTopicArgumentValueProvider("test");

        MockModelBindingContext
            .SetupGet(x => x.TopicArguments)
            .Returns(
                new ReadOnlyDictionary<string, StringValues>(
                    new Dictionary<string, StringValues>()
                )
            );

        var result = mqttTopicArgumentValueProvider.GetValueSource(MockModelBindingContext.Object);

        Assert.Equal(null, result);
    }
    */
}
