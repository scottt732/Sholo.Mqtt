using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace Sholo.Mqtt.ModelBinding.TypeConverters.Json;

[PublicAPI]
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static JsonNamingPolicy SnakeCase => InstanceFactory.Value;

    private static readonly Lazy<SnakeCaseNamingPolicy> InstanceFactory;

    static SnakeCaseNamingPolicy()
    {
        InstanceFactory = new Lazy<SnakeCaseNamingPolicy>(() => new SnakeCaseNamingPolicy());
    }

    private SnakeCaseNamingPolicy()
    {
    }

    public override string ConvertName(string? name)
    {
        return string.IsNullOrEmpty(name) ? name! : new string(ToSnakeCase(name));
    }

    // Hybrid implementation based on these (correctness of 2, perf benefits of 1):
    // - https://github.com/xsoheilalizadeh/SnakeCaseConversion/blob/master/SnakeCaseConversionBenchmark/SnakeCaseConventioneerBenchmark.cs#L49
    // - https://github.com/J0rgeSerran0/JsonNamingPolicy/blob/master/JsonSnakeCaseNamingPolicy.cs
    private ReadOnlySpan<char> ToSnakeCase(string name)
    {
        var upperCaseCushionLength = name[1..].Count(t => t is >= 'A' and <= 'Z');
        var remainingLength = name.Length - upperCaseCushionLength;
        var bufferSize = upperCaseCushionLength * 2 + remainingLength;
        var buffer = new char[bufferSize];
        var bufferPosition = 0;

        ReadOnlySpan<char> spanName = name.Trim();

        var addCharacter = true;

        var isNextLower = false;
        var isNextUpper = false;
        var isNextSpace = false;

        for (int position = 0; position < spanName.Length; position++)
        {
            if (position != 0)
            {
                var isCurrentSpace = spanName[position] == 32;
                var isPreviousSpace = spanName[position - 1] == 32;
                var isPreviousSeparator = spanName[position - 1] == 95;

                if (position + 1 != spanName.Length)
                {
                    isNextLower = spanName[position + 1] > 96 && spanName[position + 1] < 123;
                    isNextUpper = spanName[position + 1] > 64 && spanName[position + 1] < 91;
                    isNextSpace = spanName[position + 1] == 32;
                }

                if (isCurrentSpace && (isPreviousSpace || isPreviousSeparator || isNextUpper || isNextSpace))
                {
                    addCharacter = false;
                }
                else
                {
                    var isCurrentUpper = spanName[position] > 64 && spanName[position] < 91;
                    var isPreviousLower = spanName[position - 1] > 96 && spanName[position - 1] < 123;
                    var isPreviousNumber = spanName[position - 1] > 47 && spanName[position - 1] < 58;

                    if (isCurrentUpper && (isPreviousLower || isPreviousNumber || isNextLower || isNextSpace || isNextLower && !isPreviousSpace))
                    {
                        buffer[bufferPosition] = '_';
                        bufferPosition++;
                    }
                    else
                    {
                        if (isCurrentSpace && !isPreviousSpace && !isNextSpace)
                        {
                            buffer[bufferPosition] = '_';
                            bufferPosition++;
                            addCharacter = false;
                        }
                    }
                }
            }

            if (addCharacter)
            {
                var lc = char.ToLower(spanName[position], CultureInfo.InvariantCulture);
                buffer[bufferPosition] = lc;
                bufferPosition++;
            }
            else
            {
                addCharacter = true;
            }
        }

        return new Span<char>(buffer, 0, bufferPosition);
    }
}
