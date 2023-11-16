using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Sholo.Mqtt.ModelBinding.TypeConverters.Json;
using Sholo.Mqtt.Test.Specimens;
using Xunit;

namespace Sholo.Mqtt.Test.TypeConverters;

public class JsonTypeConverterTests
{
    private Mock<ILogger<JsonTypeConverter>> MockLogger { get; } = new(MockBehavior.Loose);

    [Fact]
    public void TryConvertPayload_WhenPayloadIsNotJson_ReturnsFalseWithNullResult()
    {
        var jsonTypeConverter = CreateJsonTypeConverter();
        ArraySegment<byte> payloadData = "test"u8.ToArray();

        var success = jsonTypeConverter.TryConvertPayload(payloadData, typeof(TestLight), out var light);

        Assert.False(success);
        Assert.Null(light);
    }

    [Fact]
    public void TryConvertPayload_WhenPayloadIsValidSnakeCaseJson_ReturnsTrueWithExpectedResult()
    {
        var jsonTypeConverter = CreateJsonTypeConverter(o =>
        {
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(SnakeCaseNamingPolicy.SnakeCase));
            o.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.SnakeCase;
            o.JsonSerializerOptions.DictionaryKeyPolicy = SnakeCaseNamingPolicy.SnakeCase;
        });

        ArraySegment<byte> payloadData = @"{""id"":""light.test"",""state"":""on"",""online"":true,""last_updated"":""2023-11-10T10:04:57.4007100-05:00"",""attributes"":{""test_attribute"":""abc""}}"u8.ToArray();

        var success = jsonTypeConverter.TryConvertPayload(payloadData, typeof(TestLight), out var lightObject);

        Assert.True(success);
        Assert.NotNull(lightObject);

        var light = Assert.IsType<TestLight>(lightObject);
        Assert.Equal("light.test", light.Id);
        Assert.Equal(TestLightState.On, light.State);
        Assert.Equal(true, light.Online);
        Assert.Collection(
            light.Attributes,
            kvp =>
            {
                var (key, value) = kvp;
                Assert.Equal("test_attribute", key);
                Assert.Equal("abc", value);
            }
        );
    }

    [Fact]
    public void TryConvertPayload_WhenPayloadIsValidCamelCaseJson_ReturnsTrueWithExpectedResult()
    {
        var jsonTypeConverter = CreateJsonTypeConverter(o =>
        {
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });

        ArraySegment<byte> payloadData = @"{""id"":""light.test"",""state"":""On"",""online"":true,""lastUpdated"":""2023-11-10T10:04:57.4007100-05:00"",""attributes"":{""testAttribute"": ""Abc""}}"u8.ToArray();

        var success = jsonTypeConverter.TryConvertPayload(payloadData, typeof(TestLight), out var lightObject);

        Assert.True(success);
        Assert.NotNull(lightObject);

        var light = Assert.IsType<TestLight>(lightObject);
        Assert.Equal("light.test", light.Id);
        Assert.Equal(TestLightState.On, light.State);
        Assert.Equal(true, light.Online);
        Assert.Collection(
            light.Attributes,
            kvp =>
            {
                var (key, value) = kvp;
                Assert.Equal("testAttribute", key);
                Assert.Equal("Abc", value);
            }
        );
    }

    [Fact]
    public void TryConvertPayload_WhenPayloadIsValidPascalCaseJson_ReturnsTrueWithExpectedResult()
    {
        var jsonTypeConverter = CreateJsonTypeConverter(o =>
        {
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null));
            o.JsonSerializerOptions.PropertyNamingPolicy = null;
            o.JsonSerializerOptions.DictionaryKeyPolicy = null;
        });

        ArraySegment<byte> payloadData = @"{""Id"":""light.test"",""State"":""On"",""Online"":true,""LastUpdated"":""2023-11-10T10:04:57.4007100-05:00"",""Attributes"":{""TestAttribute"": ""ABC""}}"u8.ToArray();

        var success = jsonTypeConverter.TryConvertPayload(payloadData, typeof(TestLight), out var lightObject);

        Assert.True(success);
        Assert.NotNull(lightObject);

        var light = Assert.IsType<TestLight>(lightObject);
        Assert.Equal("light.test", light.Id);
        Assert.Equal(TestLightState.On, light.State);
        Assert.Equal(true, light.Online);
        Assert.Collection(
            light.Attributes,
            kvp =>
            {
                var (key, value) = kvp;
                Assert.Equal("TestAttribute", key);
                Assert.Equal("ABC", value);
            }
        );
    }

    private JsonTypeConverter CreateJsonTypeConverter(Action<JsonTypeConverterOptions>? configure = null)
    {
        var options = new JsonTypeConverterOptions();
        configure?.Invoke(options);

        return new JsonTypeConverter(
            Options.Create(options),
            MockLogger.Object
        );
    }
}
