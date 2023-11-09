#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Sholo.Mqtt.TypeConverters;

namespace Sholo.Mqtt.ModelBinding.Context;

[PublicAPI]
public interface IParameterBindingContext : IParametersBindingContext
{
    IDictionary<ParameterInfo, object?> ActionArguments { get; }
    ParameterInfo ActionParameter { get; }
    object? Value { get; set; }
    bool TryConvert(string? input, IMqttParameterTypeConverter? explicitParameterTypeConverter, Type targetType, out object? result);
}
