using System;
using Moq;
using MQTTnet.Packets;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.ValueProviders;
using Xunit;

namespace Sholo.Mqtt.Test.ValueProviders;

public class MqttUserPropertyValueProviderTests
{
    private Mock<IParameterBindingContext> MockParameterBindingContext { get; } = new(MockBehavior.Strict);
    private Mock<IMqttRequestContext> MockMqttRequestContext { get; } = new(MockBehavior.Strict);

    private static readonly MqttUserProperty[] TestProperties = new[]
    {
        new MqttUserProperty("key1", "value1"),
        new MqttUserProperty("key2", "value2"),
        new MqttUserProperty("key2", "value3")
    };

    public MqttUserPropertyValueProviderTests()
    {
        MockParameterBindingContext
            .SetupGet(x => x.Request)
            .Returns(MockMqttRequestContext.Object);
    }

    [Theory]
    [InlineData("key1", StringComparison.Ordinal)]
    [InlineData("KEY1", StringComparison.OrdinalIgnoreCase)]
    public void GetValueSource_WhenRequestHasUserPropertyMatchkingKeyAndStringComparer_ReturnsSingleValue(string propertyName, StringComparison stringComparison)
    {
        var mqttUserPropertyValueProvider = new MqttUserPropertyValueProvider(propertyName, stringComparison);

        MockMqttRequestContext
            .SetupGet(x => x.MqttUserProperties)
            .Returns(TestProperties)
            .Verifiable(Times.Once);

        var values = mqttUserPropertyValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.Collection(
            values,
            v => Assert.Equal("value1", v)
        );
    }

    [Theory]
    [InlineData("key2", StringComparison.Ordinal)]
    [InlineData("KEY2", StringComparison.OrdinalIgnoreCase)]
    public void GetValueSource_WhenRequestHasUserPropertiesMatchkingKeyAndStringComparer_ReturnsMultipleValues(string propertyName, StringComparison stringComparison)
    {
        var mqttUserPropertyValueProvider = new MqttUserPropertyValueProvider(propertyName, stringComparison);

        MockMqttRequestContext
            .SetupGet(x => x.MqttUserProperties)
            .Returns(TestProperties)
            .Verifiable(Times.Once);

        var values = mqttUserPropertyValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.Collection(
            values,
            v => Assert.Equal("value2", v),
            v => Assert.Equal("value3", v)
        );
    }

    [Theory]
    [InlineData("key3", StringComparison.Ordinal)]
    [InlineData("KEY3", StringComparison.OrdinalIgnoreCase)]
    public void GetValueSource_WhenRequestHasUserPropertyMatchkingKeyButNotStringComparer_ReturnsEmptyArray(string propertyName, StringComparison stringComparison)
    {
        var mqttUserPropertyValueProvider = new MqttUserPropertyValueProvider(propertyName, stringComparison);

        MockMqttRequestContext
            .SetupGet(x => x.MqttUserProperties)
            .Returns(TestProperties)
            .Verifiable(Times.Once);

        var values = mqttUserPropertyValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.Empty(values);
    }
}
