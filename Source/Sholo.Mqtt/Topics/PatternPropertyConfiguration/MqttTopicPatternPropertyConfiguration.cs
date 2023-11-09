using System;
using System.Reflection;

namespace Sholo.Mqtt.Topics.PatternPropertyConfiguration;

public class MqttTopicPatternPropertyConfiguration : IMqttTopicPatternPropertyConfiguration
{
    public string ParameterName { get; }
    public bool HaveTypeConverter => TypeConverter != null;

    public object? GetParameterValue(string value)
    {
        if (HaveTypeConverter)
        {
            return TypeConverter!(value);
        }

        return null;
    }

#pragma warning disable IDE0052 // Remove unread private members - TODO: WiP

    // ReSharper disable UnusedAutoPropertyAccessor.Local

    private MethodInfo ValueSetter { get; }

    // ReSharper restore UnusedAutoPropertyAccessor.Local
#pragma warning restore IDE0052 // Remove unread private members
    private Func<string, object?>? TypeConverter { get; set; }

    public MqttTopicPatternPropertyConfiguration(
        string parameterName,
        MethodInfo valueSetter,
        Func<string, object?>? typeConverter)
    {
        ParameterName = parameterName;
        ValueSetter = valueSetter;
        TypeConverter = typeConverter;
    }
}
