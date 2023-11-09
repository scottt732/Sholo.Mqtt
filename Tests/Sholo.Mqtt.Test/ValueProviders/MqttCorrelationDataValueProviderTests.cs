using System.Linq;
using Moq;
using Sholo.Mqtt.ModelBinding.Context;
using Sholo.Mqtt.ValueProviders;
using Xunit;

namespace Sholo.Mqtt.Test.ValueProviders;

public class MqttCorrelationDataValueProviderTests
{
    private IMqttCorrelationDataValueProvider MqttCorrelationDataValueProvider { get; } = new MqttCorrelationDataValueProvider();
    private Mock<IParameterBindingContext> MockParameterBindingContext { get; } = new(MockBehavior.Strict);
    private Mock<IMqttRequestContext> MockMqttRequestContext { get; } = new(MockBehavior.Strict);

    private static readonly byte[] TestCorrelationData = { 1, 2, 3 };

    public MqttCorrelationDataValueProviderTests()
    {
        MockParameterBindingContext
            .SetupGet(x => x.Request)
            .Returns(MockMqttRequestContext.Object);
    }

    [Fact]
    public void GetValueSource_WhenRequestHasCorrelationData_ReturnsCorrelationData()
    {
        MockMqttRequestContext
            .SetupGet(x => x.CorrelationData)
            .Returns(TestCorrelationData)
            .Verifiable(Times.Once);

        var correlationData = MqttCorrelationDataValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.True(correlationData.SequenceEqual(TestCorrelationData));
    }

    [Fact]
    public void GetValueSource_WhenRequestHasCorrelationData_ReturnsNull()
    {
        MockMqttRequestContext
            .SetupGet(x => x.CorrelationData)
            .Returns(() => null)
            .Verifiable(Times.Once);

        var correlationData = MqttCorrelationDataValueProvider.GetValueSource(MockParameterBindingContext.Object);

        Assert.Null(correlationData);
    }
}
