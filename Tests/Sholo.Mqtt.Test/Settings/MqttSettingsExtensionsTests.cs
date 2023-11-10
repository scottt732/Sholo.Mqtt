using System;
using System.Text;
using MQTTnet.Protocol;
using Sholo.Mqtt.Settings;
using Xunit;

namespace Sholo.Mqtt.Test.Settings;

public class MqttSettingsExtensionsTests
{
    [Fact]
    public void GetOnlineApplicationMessage_WhenOnlineMessageIsNotNull_ReturnsNull()
    {
        var mqttSettings = new MqttSettings();

        var result = mqttSettings.GetOnlineApplicationMessage();

        Assert.Null(result);
    }

    [Fact]
    public void GetOnlineApplicationMessage_WhenMqttSettingsIsNull_ThrowsArgumentNullException()
    {
        MqttSettings mqttSettings = null!;

        Assert.Throws<ArgumentNullException>(() => mqttSettings.GetOnlineApplicationMessage());
    }

    [Theory]
    [CombinatorialData]
    public void GetOnlineApplicationMessage_WhenMqttSettingsHaveValidConfig_ReturnsExpectedResult(
        [CombinatorialValues("this/is/a/test")] string topic,
        [CombinatorialValues("payload")] string payload,
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        bool retain
    )
    {
        var mqttSettings = new MqttSettings
        {
            OnlineMessage = new MqttMessageSettings()
            {
                Topic = topic,
                Payload = payload,
                QualityOfServiceLevel = qualityOfServiceLevel,
                Retain = retain
            }
        };

        var onlineApplicationMessage = mqttSettings.GetOnlineApplicationMessage();

        Assert.NotNull(onlineApplicationMessage);

        Assert.Equal(topic, onlineApplicationMessage.Topic);
        Assert.Equal(payload, Encoding.UTF8.GetString(onlineApplicationMessage.PayloadSegment));
        Assert.Equal(qualityOfServiceLevel, onlineApplicationMessage.QualityOfServiceLevel);
        Assert.Equal(retain, onlineApplicationMessage.Retain);
    }

    [Fact]
    public void GetLastWillAndTestamentApplicationMessage_WhenOnlineMessageIsNotNull_ReturnsNull()
    {
        var mqttSettings = new MqttSettings();

        var result = mqttSettings.GetLastWillAndTestamentApplicationMessage();

        Assert.Null(result);
    }

    [Fact]
    public void GetLastWillAndTestamentApplicationMessage_WhenMqttSettingsIsNull_ThrowsArgumentNullException()
    {
        MqttSettings mqttSettings = null!;

        Assert.Throws<ArgumentNullException>(() => mqttSettings.GetLastWillAndTestamentApplicationMessage());
    }

    [Theory]
    [CombinatorialData]
    public void GetLastWillAndTestamentApplicationMessage_WhenMqttSettingsHaveValidConfig_ReturnsExpectedResult(
        [CombinatorialValues("this/is/a/test")] string topic,
        [CombinatorialValues("payload")] string payload,
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        bool retain
    )
    {
        var mqttSettings = new MqttSettings
        {
            LastWillAndTestament = new MqttMessageSettings()
            {
                Topic = topic,
                Payload = payload,
                QualityOfServiceLevel = qualityOfServiceLevel,
                Retain = retain
            }
        };

        var lastWillAndTestamentApplicationMessage = mqttSettings.GetLastWillAndTestamentApplicationMessage();

        Assert.NotNull(lastWillAndTestamentApplicationMessage);

        Assert.Equal(topic, lastWillAndTestamentApplicationMessage.Topic);
        Assert.Equal(payload, Encoding.UTF8.GetString(lastWillAndTestamentApplicationMessage.PayloadSegment));
        Assert.Equal(qualityOfServiceLevel, lastWillAndTestamentApplicationMessage.QualityOfServiceLevel);
        Assert.Equal(retain, lastWillAndTestamentApplicationMessage.Retain);
    }
}
