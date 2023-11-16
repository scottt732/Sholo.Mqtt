using System;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using Sholo.Mqtt.TypeConverters;
using Sholo.Mqtt.TypeConverters.Parameter;
using Xunit;

namespace Sholo.Mqtt.Test.TypeConverters.Parameter;
public class LambdaMqttParameterTypeConverterTests
{
    private IMqttParameterTypeConverter BrokenTypeConverter { get; } = new LambdaMqttParameterTypeConverter<bool>(s => throw new NotImplementedException("Oops"));

    private IMqttParameterTypeConverter FuzzyBooleanTypeConverter { get; } = new LambdaMqttParameterTypeConverter<bool?>(
        s =>
        {
            if (s == null)
            {
                return (true, null);
            }

            if (s.Length == 0)
            {
                return (false, null);
            }

            if (bool.TryParse(s!, out var b))
            {
                return (true, b);
            }

            if (s.Length == 1 && char.TryParse(s, out var c))
            {
                switch (c)
                {
                    case '1':
                    case 't':
                    case 'T':
                    case 'y':
                    case 'Y':
                        return (true, true);
                    case '0':
                    case 'f':
                    case 'F':
                    case 'n':
                    case 'N':
                        return (true, false);
                }
            }

            if (int.TryParse(s, out var i))
            {
                if (i <= 0)
                {
                    return (true, false);
                }
                else
                {
                    return (true, true);
                }
            }

            switch (s.ToUpperInvariant())
            {
                case "YES":
                case "TRUE":
                case "ON":
                    return (true, true);
                case "NO":
                case "FALSE":
                case "OFF":
                    return (true, false);
            }

            return (false, null);
        });

    [Fact]
    public void TryConvert_WhenInputIsNull_ReturnsTrueWithNullResult()
    {
        var success = FuzzyBooleanTypeConverter.TryConvertParameter(null, typeof(bool), out var objResult);
        Assert.True(success);
        Assert.Null(objResult);
    }

    [Fact]
    public void TryConvert_WhenInputIsEmptyString_ReturnsFalseWithNullResult()
    {
        var success = FuzzyBooleanTypeConverter.TryConvertParameter(string.Empty, typeof(bool), out var objResult);
        Assert.False(success);
        Assert.Null(objResult);
    }

    [Theory]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("TrUe")]
    [InlineData("1")]
    [InlineData("t")]
    [InlineData("T")]
    [InlineData("y")]
    [InlineData("Y")]
    [InlineData("2")]
    [InlineData("YES")]
    [InlineData("yes")]
    [InlineData("YeS")]
    [InlineData("ON")]
    [InlineData("On")]
    [InlineData("oN")]

    public void TryConvert_WhenInputIsTruthy_ReturnsTrueWithTrueResult(string value)
    {
        var success = FuzzyBooleanTypeConverter.TryConvertParameter(value, typeof(bool), out var objResult);

        Assert.True(success);
        var result = Assert.IsAssignableFrom<bool?>(objResult);
        Assert.True(result);
    }

    [Theory]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("FaLsE")]
    [InlineData("0")]
    [InlineData("f")]
    [InlineData("F")]
    [InlineData("n")]
    [InlineData("N")]
    [InlineData("-1")]
    [InlineData("NO")]
    [InlineData("no")]
    [InlineData("No")]
    [InlineData("OFF")]
    [InlineData("Off")]
    [InlineData("oFf")]

    public void TryConvert_WhenInputIsFalsey_ReturnsTrueWithFalseResult(string value)
    {
        var success = FuzzyBooleanTypeConverter.TryConvertParameter(value, typeof(bool), out var objResult);

        Assert.True(success);
        var result = Assert.IsAssignableFrom<bool?>(objResult);
        Assert.False(result);
    }

    [Theory]
    [InlineData("widget")]
    [InlineData("{0123}")]

    public void TryConvert_WhenInputIsSomethingElse_ReturnsFalseWithNullResult(string value)
    {
        var success = FuzzyBooleanTypeConverter.TryConvertParameter(value, typeof(bool), out var objResult);

        Assert.False(success);
        Assert.Null(objResult);
    }

    [Fact]
    public void TryConvert_WhenTypeConverterThrows_ReturnsFalseWithNullResult()
    {
        var success = BrokenTypeConverter.TryConvertParameter("abc", typeof(bool), out var objResult);

        Assert.False(success);
        Assert.Null(objResult);
    }
}
