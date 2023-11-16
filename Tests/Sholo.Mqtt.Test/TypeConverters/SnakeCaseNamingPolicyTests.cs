using Sholo.Mqtt.ModelBinding.TypeConverters.Json;
using Xunit;

namespace Sholo.Mqtt.Test.TypeConverters;

public class SnakeCaseNamingPolicyTests
{
    [Fact]
    public void ConvertName_InputIsNull_ReturnsNull()
    {
        Assert.Equal(null, SnakeCaseNamingPolicy.SnakeCase.ConvertName(null!));
    }

    [Fact]
    public void ConvertName_InputIsEmptyString_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, SnakeCaseNamingPolicy.SnakeCase.ConvertName(string.Empty));
    }

    [Fact]
    public void ConvertName_InputIsValid_ReturnsExpectedResult()
    {
        Assert.Equal("a", SnakeCaseNamingPolicy.SnakeCase.ConvertName("A"));
        Assert.Equal("1", SnakeCaseNamingPolicy.SnakeCase.ConvertName("1"));
        Assert.Equal("this_is_a_test", SnakeCaseNamingPolicy.SnakeCase.ConvertName("ThisIsATest"));
        Assert.Equal("this_is_a_test", SnakeCaseNamingPolicy.SnakeCase.ConvertName("This is a test"));
        Assert.Equal("this_is_a_test", SnakeCaseNamingPolicy.SnakeCase.ConvertName("This Is A Test"));
        Assert.Equal("whoa_wtf", SnakeCaseNamingPolicy.SnakeCase.ConvertName("WhoaWtf"));
        Assert.Equal("whoa_wtf", SnakeCaseNamingPolicy.SnakeCase.ConvertName("WhoaWTF"));
        Assert.Equal("now_i_know_my_ab_cs", SnakeCaseNamingPolicy.SnakeCase.ConvertName("NowIKnowMyABCs"));
    }
}
