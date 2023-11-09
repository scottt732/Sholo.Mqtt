using System;
using System.Text;
using MQTTnet.Protocol;
using Sholo.Mqtt.Settings;
using Xunit;

namespace Sholo.Mqtt.Test.Settings;

public class MqttMessageSettingsExtensionsTests
{
    [Fact]
    public void ToMqttApplicationMessage_WhenMessageIsNull_ThrowsArgumentNullException()
    {
        MqttMessageSettings input = null!;

        // ReSharper disable once ExpressionIsAlwaysNull
        Assert.Throws<ArgumentNullException>(() => input.ToMqttApplicationMessage());
    }

    [Fact]
    public void ToMqttApplicationMessage_WhenMessageDoesNotSupplyDefaultValues_ReturnsExpectedResult()
    {
        var mqttMessageSettings = new MqttMessageSettings
        {
            Topic = "this/is/a/test",
            Payload = "test"
        };

        var mqttApplicationMessage = mqttMessageSettings.ToMqttApplicationMessage();

        Assert.Equal("this/is/a/test", mqttApplicationMessage.Topic);

        var payload = Encoding.UTF8.GetString(mqttApplicationMessage.PayloadSegment);
        Assert.Equal("test", payload);

        Assert.Equal(MqttQualityOfServiceLevel.AtMostOnce, mqttApplicationMessage.QualityOfServiceLevel);
        Assert.Equal(false, mqttApplicationMessage.Retain);
    }

    [Fact]
    public void ToMqttApplicationMessage_WhenMessageIsValid_ReturnsExpectedResult()
    {
        var mqttMessageSettings = new MqttMessageSettings
        {
            Topic = "this/is/a/test",
            Payload = "test",
            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
            Retain = true
        };

        var mqttApplicationMessage = mqttMessageSettings.ToMqttApplicationMessage();

        Assert.Equal("this/is/a/test", mqttApplicationMessage.Topic);

        var payload = Encoding.UTF8.GetString(mqttApplicationMessage.PayloadSegment);
        Assert.Equal("test", payload);

        Assert.Equal(MqttQualityOfServiceLevel.ExactlyOnce, mqttApplicationMessage.QualityOfServiceLevel);
        Assert.Equal(true, mqttApplicationMessage.Retain);
    }
}
