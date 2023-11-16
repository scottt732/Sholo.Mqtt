using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using Sholo.Mqtt.ModelBinding.TypeConverters.Attributes;
using Sholo.Mqtt.Topics.Filter;

namespace Sholo.Mqtt.ModelBinding;

public enum ParameterValidationResult
{
    NotYetValidated,
    Valid,
    Invalid,
    ValidationSuppressed
}

[PublicAPI]
public class ParameterState
{
    public MqttBindingSource? BindingSource { get; private set; }
    public bool IsModelSet { get; private set; }

    public object? Value { get; private set; }
    public ParameterInfo ParameterInfo { get; }
    public string? ParameterName => ParameterInfo.Name!;
    public Type TargetType => ParameterInfo.ParameterType;

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Value" /> should be validated.
    /// </summary>
    public bool SuppressValidation => ValidationStatus == ParameterValidationResult.ValidationSuppressed;
    public ParameterValidationResult ValidationStatus { get; private set; } = ParameterValidationResult.NotYetValidated;
    public ValidationResult[]? ValidationResults { get; private set; }

    private IMqttModelBindingContext ModelBindingContext { get; }

    internal void SetBindingSuccess(object? value)
    {
        Value = value;
        IsModelSet = true;
    }

    internal void SetBindingFailure()
    {
        Value = null;
        IsModelSet = false;
    }

    internal void SetValidationSuppressed()
    {
        if (ValidationResults != null)
        {
            throw new InvalidOperationException("The validation has already occurred.");
        }

        ValidationStatus = ParameterValidationResult.ValidationSuppressed;
    }

    internal void SetValidationResults(IEnumerable<ValidationResult> validationResults)
    {
        if (validationResults == null)
        {
            throw new ArgumentNullException(nameof(validationResults));
        }

        ValidationResults = validationResults.ToArray();
        ValidationStatus = ValidationResults.Length == 0 ? ParameterValidationResult.Valid : ParameterValidationResult.Invalid;
    }

    public ParameterState(IMqttModelBindingContext modelBindingContext, ParameterInfo parameterInfo)
    {
        ModelBindingContext = modelBindingContext;
        ParameterInfo = parameterInfo;
    }

    private IMqttCorrelationDataTypeConverter GetTypeConverter()
    {
        if (TryGetTypeConverter<FromMqttCorrelationDataAttribute, IMqttCorrelationDataTypeConverter>(tc => tc.TypeConverterType, out _))
        {
            BindingSource = MqttBindingSource.CorrelationData;
        }
        else if (TryGetTypeConverter<FromMqttPayloadAttribute, IMqttPayloadTypeConverter>(tc => tc.TypeConverterType, out _))
        {
            BindingSource = MqttBindingSource.Payload;
        }
        else if (TryGetTypeConverter<FromMqttTopicAttribute, IMqttParameterTypeConverter>(tc => tc.TypeConverterType, out _))
        {
            BindingSource = MqttBindingSource.Topic;
        }
        else if (TargetType == typeof(IMqttRequestContext))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request);
            SetValidationSuppressed();
        }
        else if (TargetType == typeof(CancellationToken))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request.ShutdownToken);
            SetValidationSuppressed();
        }
        else if (TargetType == typeof(IServiceProvider))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request.ServiceProvider);
            SetValidationSuppressed();
        }
        else if (TargetType == typeof(IMqttTopicFilter))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.TopicFilter);
            SetValidationSuppressed();
        }
        else if (TargetType == typeof(string) && (ParameterName?.Equals("topic", StringComparison.Ordinal) ?? false))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request.Topic);
        }
        else if (TargetType == typeof(byte[]) && (ParameterName?.Equals("correlationData", StringComparison.Ordinal) ?? false))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request.CorrelationData);
        }
        else if (TargetType == typeof(byte[]) && (ParameterName?.Equals("payload", StringComparison.Ordinal) ?? false))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request.Payload.ToArray());
        }
        else if (TargetType == typeof(ArraySegment<byte>) && (ParameterName?.Equals("payload", StringComparison.Ordinal) ?? false))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request.Payload);
        }
        else if (TargetType == typeof(ArraySegment<byte>) && (ParameterName?.Equals("payload", StringComparison.Ordinal) ?? false))
        {
            BindingSource = MqttBindingSource.Context;
            SetBindingSuccess(ModelBindingContext.Request.);
        }
        else
        {
            // Need to fall back to "figure-it-out" mode
            // MqttBindingSource.Topic
            // MqttBindingSource.Context
            // MqttBindingSource.CorrelationData
            // MqttBindingSource.Payload
            // MqttBindingSource.Services
            // MqttBindingSource.UserProperties

        }
    }

    private bool TryGetTypeConverter<TAttribute, TTypeConverter>(
        Func<TAttribute, Type?> typeConverterTypeSelector,
        [MaybeNullWhen(false)] out TTypeConverter typeConverter
    )
        where TAttribute : Attribute
        where TTypeConverter : class
    {
        var customAttribute = ParameterInfo.GetCustomAttribute<TAttribute>();
        if (customAttribute == null)
        {
            typeConverter = null!;
            return false;
        }

        var explicitTypeConverterType = typeConverterTypeSelector.Invoke(customAttribute);
        if (explicitTypeConverterType == null)
        {
            typeConverter = null!;
            return false;
        }

        var serviceProvider = ModelBindingContext.Request.ServiceProvider;

        var typeConverterInstance = serviceProvider.GetService(explicitTypeConverterType) ??
                                    Activator.CreateInstance(explicitTypeConverterType);

        if (typeConverterInstance is not TTypeConverter validTypeConverter)
        {
            throw new InvalidOperationException($"Unable to resolve {explicitTypeConverterType.Name}");
        }

        typeConverter = validTypeConverter;
        return true;
    }
}
