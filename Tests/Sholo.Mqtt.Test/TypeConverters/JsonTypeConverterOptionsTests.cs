using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sholo.Mqtt.ModelBinding.TypeConverters.Json;
using Xunit;

namespace Sholo.Mqtt.Test.TypeConverters;

public class JsonTypeConverterOptionsTests
{
    [Fact]
    public void JsonTypeConverterOptions_WhenBoundToConfiguration_CreatesValidObjects()
    {
        var staticConfiguration = new Dictionary<string, string?>
        {
            ["Test:JsonReaderOptions:CommentHandling"] = "Allow",
            ["Test:JsonReaderOptions:MaxDepth"] = "42",
            ["Test:JsonReaderOptions:AllowTrailingCommas"] = "true",
            ["Test:JsonSerializerOptions:AllowTrailingCommas"] = "true",
            ["Test:JsonSerializerOptions:DefaultBufferSize"] = "32767",
            ["Test:JsonSerializerOptions:DefaultIgnoreCondition"] = "WhenWritingDefault",
            ["Test:JsonSerializerOptions:IgnoreReadOnlyFields"] = "true",
            ["Test:JsonSerializerOptions:IgnoreReadOnlyProperties"] = "true",
            ["Test:JsonSerializerOptions:IncludeFields"] = "true",
            ["Test:JsonSerializerOptions:MaxDepth"] = "42",
            ["Test:JsonSerializerOptions:NumberHandling"] = "AllowReadingFromString,AllowNamedFloatingPointLiterals",
            ["Test:JsonSerializerOptions:PropertyNameCaseInsensitive"] = "true",
            ["Test:JsonSerializerOptions:ReadCommentHandling"] = "Disallow",
            ["Test:JsonSerializerOptions:UnknownTypeHandling"] = "JsonNode",
            ["Test:JsonSerializerOptions:WriteIndented"] = "true",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(staticConfiguration)
            .Build();

        var defaultJsonTypeInfoResolver = new DefaultJsonTypeInfoResolver();
        var jsonStringEnumConverter = new JsonStringEnumConverter();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        serviceCollection.AddSingleton<JsonConverter>(jsonStringEnumConverter);
        serviceCollection.AddSingleton(JsonNamingPolicy.CamelCase);
        serviceCollection.AddSingleton(ReferenceHandler.IgnoreCycles);
        serviceCollection.AddSingleton(JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
        serviceCollection.AddSingleton<IJsonTypeInfoResolver>(defaultJsonTypeInfoResolver);

        serviceCollection.AddOptions<JsonTypeConverterOptions>()
            .Configure<IServiceProvider>((opt, sp) =>
            {
                var jsonConverters = sp.GetRequiredService<IEnumerable<JsonConverter>>();
                foreach (var jsonConverter in jsonConverters)
                {
                    opt.JsonSerializerOptions.Converters.Add(jsonConverter);
                }

                var jsonNamingPolicy = sp.GetRequiredService<JsonNamingPolicy>();
                opt.JsonSerializerOptions.DictionaryKeyPolicy = jsonNamingPolicy;
                opt.JsonSerializerOptions.PropertyNamingPolicy = jsonNamingPolicy;

                var javaScriptEncoder = sp.GetRequiredService<JavaScriptEncoder>();
                opt.JsonSerializerOptions.Encoder = javaScriptEncoder;

                var referenceHandler = sp.GetRequiredService<ReferenceHandler>();
                opt.JsonSerializerOptions.ReferenceHandler = referenceHandler;

                var jsonTypeInfoResolver = sp.GetRequiredService<IJsonTypeInfoResolver>();
                opt.JsonSerializerOptions.TypeInfoResolver = jsonTypeInfoResolver;
            })
            .BindConfiguration("test");

        var services = serviceCollection.BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<JsonTypeConverterOptions>>();

        Assert.Equal(JsonCommentHandling.Allow, options.Value.JsonReaderOptions.CommentHandling);
        Assert.Equal(42, options.Value.JsonReaderOptions.MaxDepth);
        Assert.True(options.Value.JsonReaderOptions.AllowTrailingCommas);

        Assert.True(options.Value.JsonSerializerOptions.AllowTrailingCommas);
        Assert.Equal(32767, options.Value.JsonSerializerOptions.DefaultBufferSize);
        Assert.Equal(JsonIgnoreCondition.WhenWritingDefault, options.Value.JsonSerializerOptions.DefaultIgnoreCondition);
        Assert.True(options.Value.JsonSerializerOptions.IgnoreReadOnlyFields);
        Assert.True(options.Value.JsonSerializerOptions.IgnoreReadOnlyProperties);
        Assert.True(options.Value.JsonSerializerOptions.IncludeFields);
        Assert.Equal(42, options.Value.JsonSerializerOptions.MaxDepth);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals, options.Value.JsonSerializerOptions.NumberHandling);
        Assert.True(options.Value.JsonSerializerOptions.NumberHandling.HasFlag(JsonNumberHandling.AllowReadingFromString));
        Assert.False(options.Value.JsonSerializerOptions.NumberHandling.HasFlag(JsonNumberHandling.WriteAsString));
        Assert.True(options.Value.JsonSerializerOptions.NumberHandling.HasFlag(JsonNumberHandling.AllowNamedFloatingPointLiterals));
        Assert.True(options.Value.JsonSerializerOptions.PropertyNameCaseInsensitive);
        Assert.Equal(JsonCommentHandling.Disallow, options.Value.JsonSerializerOptions.ReadCommentHandling);
        Assert.Equal(JsonUnknownTypeHandling.JsonNode, options.Value.JsonSerializerOptions.UnknownTypeHandling);
        Assert.True(options.Value.JsonSerializerOptions.WriteIndented);

        Assert.Collection(
            options.Value.JsonSerializerOptions.Converters,
            c => Assert.Same(jsonStringEnumConverter, c)
        );

        Assert.Same(JsonNamingPolicy.CamelCase, options.Value.JsonSerializerOptions.DictionaryKeyPolicy);
        Assert.Same(JsonNamingPolicy.CamelCase, options.Value.JsonSerializerOptions.PropertyNamingPolicy);

        Assert.Same(JavaScriptEncoder.UnsafeRelaxedJsonEscaping, options.Value.JsonSerializerOptions.Encoder);

        Assert.Same(ReferenceHandler.IgnoreCycles, options.Value.JsonSerializerOptions.ReferenceHandler);

        Assert.Same(defaultJsonTypeInfoResolver, options.Value.JsonSerializerOptions.TypeInfoResolver);
    }
}
