using MQTTnet.Protocol;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.Utilities;
using Xunit;

namespace Sholo.Mqtt.Test.Settings;

public class MqttMessageSettingsTests
{
    [Fact]
    public void Validate_WhenMessageIsValid_ReturnsNoValidationErrors()
    {
        var mqttMessageSettings = new MqttMessageSettings
        {
            Topic = "this/is/a/test",
            Payload = "test",
            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
            Retain = true
        };

        ValidationHelper.TryValidateObject(mqttMessageSettings, out var validationResults);

        Assert.Empty(validationResults);
    }

    [Fact]
    public void Validate_WhenTopicIsNull_ReturnsExpectedValidationErrors()
    {
        var mqttMessageSettings = new MqttMessageSettings
        {
            Topic = null!,
            Payload = "test",
            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
            Retain = true
        };

        ValidationHelper.TryValidateObject(mqttMessageSettings, out var validationResults);

        Assert.Collection(
            validationResults,
            validationResult => Assert.Equal("Topic is required.", validationResult.ErrorMessage)
        );
    }

    [Fact]
    public void Validate_WhenTopicIsEmpty_ReturnsExpectedValidationErrors()
    {
        var mqttMessageSettings = new MqttMessageSettings
        {
            Topic = string.Empty,
            Payload = "test",
            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
            Retain = true
        };

        ValidationHelper.TryValidateObject(mqttMessageSettings, out var validationResults);

        Assert.Collection(
            validationResults,
            validationResult => Assert.Equal("Topic can not be empty.", validationResult.ErrorMessage)
        );
    }

    [Theory]
    [InlineData('+')]
    [InlineData('#')]
    public void Validate_WhenTopicContainsInvalidCharacters_ReturnsExpectedValidationErrors(char invalidCharacter)
    {
        var mqttMessageSettings = new MqttMessageSettings
        {
            Topic = $"this/{invalidCharacter}/test",
            Payload = "test",
            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
            Retain = true
        };

        ValidationHelper.TryValidateObject(mqttMessageSettings, out var validationResults);

        Assert.Collection(
            validationResults,
            validationResult => Assert.Equal("The characters '+' and '#' are not allowed in topics.", validationResult.ErrorMessage)
        );
    }

    [Fact]
    public void Validate_WhenQualityServiceLevelIsInvalid_ReturnsExpectedValidationErrors()
    {
        var mqttMessageSettings = new MqttMessageSettings
        {
            Topic = "this/is/a/test",
            Payload = "test",
            QualityOfServiceLevel = (MqttQualityOfServiceLevel)1024,
            Retain = true
        };

        ValidationHelper.TryValidateObject(mqttMessageSettings, out var validationResults);

        Assert.Collection(
            validationResults,
            validationResult => Assert.Equal($"Invalid {nameof(mqttMessageSettings.QualityOfServiceLevel)} value: 1024", validationResult.ErrorMessage)
        );
    }
}
