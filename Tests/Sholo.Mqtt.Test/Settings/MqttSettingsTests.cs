using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MQTTnet.Protocol;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.Utilities;
using Xunit;

namespace Sholo.Mqtt.Test.Settings;

public class MqttSettingsTests
{
    [Fact]
    public void Validate_WhenSettingsAreValid_ReturnsNoValidationErrors()
    {
        var mqttSettings = new MqttSettings
        {
            Host = "localhost",
            OnlineMessage = new MqttMessageSettings
            {
                Topic = "this/is/a/test",
                Payload = "online",
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
                Retain = true
            },
            LastWillAndTestament = new MqttMessageSettings
            {
                Topic = "this/is/a/test",
                Payload = "offline",
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
                Retain = true
            }
        };

        ValidationHelper.TryValidateObject(mqttSettings, out var validationResults);

        Assert.Empty(validationResults);
    }

    [Fact]
    public void Validate_WhenOnlineMessageIsInvalid_ReturnsExpectedValidationErrors()
    {
        var mqttSettings = new MqttSettings
        {
            Host = "localhost",
            OnlineMessage = new MqttMessageSettings
            {
                Topic = "#",
                Payload = null!,
                QualityOfServiceLevel = (MqttQualityOfServiceLevel)1024,
            },
            LastWillAndTestament = new MqttMessageSettings
            {
                Topic = "this/is/a/test",
                Payload = "offline",
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
                Retain = true
            }
        };

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.ValidateObject(mqttSettings, validationResults);

        Assert.False(success);
        Assert.Equal(3, validationResults.Count);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(MqttSettings.OnlineMessage)}.{nameof(MqttMessageSettings.Topic)}", m));
                Assert.Equal("The characters '+' and '#' are not allowed in topics.", v.ErrorMessage);
            },
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(MqttSettings.OnlineMessage)}.{nameof(MqttMessageSettings.QualityOfServiceLevel)}", m));
                Assert.Equal("Invalid QualityOfServiceLevel value: 1024", v.ErrorMessage);
            },
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(MqttSettings.OnlineMessage)}.{nameof(MqttMessageSettings.Payload)}", m));
                Assert.Equal($"{nameof(MqttMessageSettings.Payload)} is required.", v.ErrorMessage);
            }
        );
    }
}
