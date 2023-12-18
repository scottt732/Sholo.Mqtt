namespace Sholo.Mqtt.Test.ValueProviders;

/*
public class MqttPayloadValueProviderTests
{
    private IMqttPayloadValueProvider MqttPayloadValueProvider { get; } = new MqttPayloadValueProvider();
    private Mock<IMqttModelBindingContext> MockModelBindingContext { get; } = new(MockBehavior.Strict);
    private Mock<IMqttRequestContext> MockMqttRequestContext { get; } = new(MockBehavior.Strict);

    private static readonly ArraySegment<byte> TestPayload = new(
        new byte[] { 0x01, 0x02, 0x03 }
    );

    public MqttPayloadValueProviderTests()
    {
        MockModelBindingContext
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

        var payload = MqttPayloadValueProvider.GetValueSource(MockModelBindingContext.Object);

        Assert.True(payload.SequenceEqual(TestPayload));
    }

    [Fact]
    public void GetValueSource_WhenRequestHasCorrelationData_ReturnsNull()
    {
        MockMqttRequestContext
            .SetupGet(x => x.Payload)
            .Returns(() => null!)
            .Verifiable(Times.Once);

        var payload = MqttPayloadValueProvider.GetValueSource(MockModelBindingContext.Object);

        Assert.Equal(0, payload.Count);
    }
}
*/
