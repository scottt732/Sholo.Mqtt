using System.Collections.Generic;
using System.Linq;
using Moq;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.ValueProviders;
using Xunit;

namespace Sholo.Mqtt.Test.ValueProviders;

public class MqttTopicArgumentValueProviderTests
{
    private Mock<IParameterBindingContext> MockParameterBindingContext { get; } = new(MockBehavior.Strict);

    [Fact]
    public void GetValueSource_WhenTopicHasParameter_ReturnsValue()
    {
        var mqttTopicArgumentValueProvider = new MqttTopicArgumentValueProvider("test");

        var expectedResults = new[] { "result" };

        MockParameterBindingContext
            .SetupGet(x => x.TopicArguments)
            .Returns(new Dictionary<string, string[]>
            {
                ["test"] = expectedResults
            });

        var results = mqttTopicArgumentValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.NotNull(results);
        Assert.True(results.SequenceEqual(expectedResults));
    }

    [Fact]
    public void GetValueSource_WhenTopicHasParameter_ReturnsNull()
    {
        var mqttTopicArgumentValueProvider = new MqttTopicArgumentValueProvider("test");

        MockParameterBindingContext
            .SetupGet(x => x.TopicArguments)
            .Returns(new Dictionary<string, string[]>());

        var result = mqttTopicArgumentValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.Equal(null, result);
    }
}
