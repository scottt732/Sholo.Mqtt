using System;
using System.Linq;
using Moq;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.ValueProviders;
using Xunit;

namespace Sholo.Mqtt.Test.ValueProviders;

public class MqttPayloadValueProviderTests
{
    private IMqttPayloadValueProvider MqttPayloadValueProvider { get; } = new MqttPayloadValueProvider();
    private Mock<IParameterBindingContext> MockParameterBindingContext { get; } = new(MockBehavior.Strict);
    private Mock<IMqttRequestContext> MockMqttRequestContext { get; } = new(MockBehavior.Strict);

    private static readonly ArraySegment<byte> TestPayload = new(
        new byte[] { 0x01, 0x02, 0x03 }
    );

    public MqttPayloadValueProviderTests()
    {
        MockParameterBindingContext
            .SetupGet(x => x.Request)
            .Returns(MockMqttRequestContext.Object);
    }

    [Fact]
    public void GetValueSource_WhenRequestHasCorrelationData_ReturnsCorrelationData()
    {
        MockMqttRequestContext
            .SetupGet(x => x.Payload)
            .Returns(TestPayload)
            .Verifiable(Times.Once);

        var payload = MqttPayloadValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.True(payload.SequenceEqual(TestPayload));
    }

    [Fact]
    public void GetValueSource_WhenRequestHasCorrelationData_ReturnsNull()
    {
        MockMqttRequestContext
            .SetupGet(x => x.Payload)
            .Returns(() => null)
            .Verifiable(Times.Once);

        var payload = MqttPayloadValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.Equal(0, payload.Count);
    }
}
