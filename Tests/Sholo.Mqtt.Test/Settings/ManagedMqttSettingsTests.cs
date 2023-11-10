using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.Utilities;
using Xunit;

namespace Sholo.Mqtt.Test.Settings;

public class ManagedMqttSettingsTests : BaseMqttSettingsTests<ManagedMqttSettings>
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_WhenMaxPendingMessagesIsLessThanOrEqualToZero_ReturnsExpectedValidationError(int maxPendingMessages)
    {
        var mqttSettings = new ManagedMqttSettings
        {
            Host = "localhost",
            MaxPendingMessages = maxPendingMessages
        };

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(ManagedMqttSettings.MaxPendingMessages)}", m));
                Assert.StartsWith($"{nameof(ManagedMqttSettings.MaxPendingMessages)} must be greater than 0.", v.ErrorMessage, StringComparison.Ordinal);
            }
        );
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-1)]
    public void Validate_WhenAutoReconnectDelayIsNegative_ReturnsExpectedValidationError(int seconds)
    {
        var mqttSettings = new ManagedMqttSettings
        {
            Host = "localhost",
            AutoReconnectDelay = TimeSpan.FromSeconds(seconds)
        };

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(ManagedMqttSettings.AutoReconnectDelay)}", m));
                Assert.StartsWith($"{nameof(ManagedMqttSettings.AutoReconnectDelay)} must be greater than or equal to 0.", v.ErrorMessage, StringComparison.Ordinal);
            }
        );
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-1)]
    public void Validate_WhenMaxTopicFiltersInSubscribeUnsubscribePacketsIsNegative_ReturnsExpectedValidationError(int maxTopicFiltersInSubscribeUnsubscribePackets)
    {
        var mqttSettings = new ManagedMqttSettings
        {
            Host = "localhost",
            MaxTopicFiltersInSubscribeUnsubscribePackets = maxTopicFiltersInSubscribeUnsubscribePackets
        };

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(ManagedMqttSettings.MaxTopicFiltersInSubscribeUnsubscribePackets)}", m));
                Assert.StartsWith($"{nameof(ManagedMqttSettings.MaxTopicFiltersInSubscribeUnsubscribePackets)} must be greater than or equal to 0.", v.ErrorMessage, StringComparison.Ordinal);
            }
        );
    }

    [Theory]
    [CombinatorialData]
    public void Validate_WhenManagedSettingsAreValid_ReturnsTrueWithNullValidationResults(
        [CombinatorialValues(1, 2)] int maxPendingMessages,
        [CombinatorialValues(0, 1)] int autoReconnectDelaySeconds,
        [CombinatorialValues(0, 1)] int maxTopicFiltersInSubscribeUnsubscribePackets
    )
    {
        var mqttSettings = new ManagedMqttSettings
        {
            Host = "localhost",
            MaxPendingMessages = maxPendingMessages,
            AutoReconnectDelay = TimeSpan.FromSeconds(autoReconnectDelaySeconds),
            MaxTopicFiltersInSubscribeUnsubscribePackets = maxTopicFiltersInSubscribeUnsubscribePackets
        };

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults);

        Assert.True(success);
        Assert.Empty(validationResults);
    }
}
