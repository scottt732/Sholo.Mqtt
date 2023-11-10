using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Moq;
using MQTTnet.Protocol;
using Sholo.Mqtt.Settings;
using Sholo.Mqtt.Utilities;
using Xunit;

namespace Sholo.Mqtt.Test.Settings;

public abstract class BaseMqttSettingsTests<TMqttSettings>
    where TMqttSettings : MqttSettings, new()
{
    [Fact]
    public void Validate_WhenSettingsAreValid_ReturnsNoValidationErrors()
    {
        var mqttSettings = new TMqttSettings
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

        var success = ValidationHelper.IsValid(mqttSettings, out var validationResults);

        Assert.True(success);
        Assert.Null(validationResults);
    }

    [Fact]
    public void Validate_WhenOnlineMessageIsInvalid_ReturnsExpectedValidationErrors()
    {
        var mqttSettings = new TMqttSettings
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
        var success = ValidationHelper.IsValid(mqttSettings, validationResults);

        Assert.False(success);
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

    [Fact]
    public void Validate_WhenLastWillAndTestamentMessageIsInvalid_ReturnsExpectedValidationErrors()
    {
        var mqttSettings = new TMqttSettings
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
                Topic = "#",
                Payload = null!,
                QualityOfServiceLevel = (MqttQualityOfServiceLevel)1024,
            }
        };

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(MqttSettings.LastWillAndTestament)}.{nameof(MqttMessageSettings.Topic)}", m));
                Assert.Equal("The characters '+' and '#' are not allowed in topics.", v.ErrorMessage);
            },
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(MqttSettings.LastWillAndTestament)}.{nameof(MqttMessageSettings.QualityOfServiceLevel)}", m));
                Assert.Equal("Invalid QualityOfServiceLevel value: 1024", v.ErrorMessage);
            },
            v =>
            {
                Assert.Collection(v.MemberNames, m => Assert.Equal($"{nameof(MqttSettings.LastWillAndTestament)}.{nameof(MqttMessageSettings.Payload)}", m));
                Assert.Equal($"{nameof(MqttMessageSettings.Payload)} is required.", v.ErrorMessage);
            }
        );
    }

    [Theory]
    [InlineData("pub.pem", null)]
    [InlineData(null, "priv.pem")]
    public void Validate_WhenOneOfClientCertificatePublicKeyPemFileAndClientCertificatePrivateKeyPemFileIsNull_ReturnsExpectedValidationError(string? publicKeyPemFile, string? privateKeyPemFile)
    {
        var mqttSettings = new TMqttSettings
        {
            Host = "localhost",
            ClientCertificatePublicKeyPemFile = publicKeyPemFile,
            ClientCertificatePrivateKeyPemFile = privateKeyPemFile
        };

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(
                    v.MemberNames,
                    m => Assert.Equal($"{nameof(MqttSettings.ClientCertificatePublicKeyPemFile)}", m),
                    m => Assert.Equal($"{nameof(MqttSettings.ClientCertificatePrivateKeyPemFile)}", m)
                );
                Assert.Equal("Client certificate public key and private key must both be specified or both be null", v.ErrorMessage);
            }
        );
    }

    [Fact]
    public void Validate_WhenClientCertificatePublicKeyPemFileDoesNotExist_ReturnsExpectedValidationError()
    {
        var mqttSettings = new TMqttSettings
        {
            Host = "localhost",
            ClientCertificatePublicKeyPemFile = "public.pem",
            ClientCertificatePrivateKeyPemFile = "private.pem"
        };

        var mockFileAbstraction = new Mock<IFileAbstraction>();

        mockFileAbstraction
            .Setup(x => x.Exists("public.pem"))
            .Returns(true);

        mockFileAbstraction
            .Setup(x => x.Exists("private.pem"))
            .Returns(false);

        var fileAbstraction = mockFileAbstraction.Object;

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults, fileAbstraction);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(
                    v.MemberNames,
                    m => Assert.Equal($"{nameof(MqttSettings.ClientCertificatePrivateKeyPemFile)}", m)
                );
                Assert.Equal("The private key file specified does not exist", v.ErrorMessage);
            }
        );
    }

    [Fact]
    public void Validate_WhenClientCertificatePrivateKeyPemFileDoesNotExist_ReturnsExpectedValidationError()
    {
        var mqttSettings = new TMqttSettings
        {
            Host = "localhost",
            ClientCertificatePublicKeyPemFile = "public.pem",
            ClientCertificatePrivateKeyPemFile = "private.pem"
        };

        var mockFileAbstraction = new Mock<IFileAbstraction>();

        mockFileAbstraction
            .Setup(x => x.Exists("public.pem"))
            .Returns(false);

        mockFileAbstraction
            .Setup(x => x.Exists("private.pem"))
            .Returns(true);

        var fileAbstraction = mockFileAbstraction.Object;

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults, fileAbstraction);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(
                    v.MemberNames,
                    m => Assert.Equal($"{nameof(MqttSettings.ClientCertificatePublicKeyPemFile)}", m)
                );
                Assert.Equal("The public key file specified does not exist", v.ErrorMessage);
            }
        );
    }

    [Fact]
    public void Validate_WhenClientCertificatePrivateKeyAndPublicKeyPemFilesDoNotExist_ReturnsExpectedValidationError()
    {
        var mqttSettings = new TMqttSettings
        {
            Host = "localhost",
            ClientCertificatePublicKeyPemFile = "public.pem",
            ClientCertificatePrivateKeyPemFile = "private.pem"
        };

        var mockFileAbstraction = new Mock<IFileAbstraction>();

        mockFileAbstraction
            .Setup(x => x.Exists("public.pem"))
            .Returns(false);

        mockFileAbstraction
            .Setup(x => x.Exists("private.pem"))
            .Returns(false);

        var fileAbstraction = mockFileAbstraction.Object;

        var validationResults = new List<ValidationResult>();
        var success = ValidationHelper.IsValid(mqttSettings, validationResults, fileAbstraction);

        Assert.False(success);
        Assert.Collection(
            validationResults,
            v =>
            {
                Assert.Collection(
                    v.MemberNames,
                    m => Assert.Equal($"{nameof(MqttSettings.ClientCertificatePublicKeyPemFile)}", m)
                );
                Assert.Equal("The public key file specified does not exist", v.ErrorMessage);
            },
            v =>
            {
                Assert.Collection(
                    v.MemberNames,
                    m => Assert.Equal($"{nameof(MqttSettings.ClientCertificatePrivateKeyPemFile)}", m)
                );
                Assert.Equal("The private key file specified does not exist", v.ErrorMessage);
            }
        );
    }
}
