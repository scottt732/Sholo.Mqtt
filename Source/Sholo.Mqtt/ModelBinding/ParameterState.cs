using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Sholo.Mqtt.ModelBinding;

[PublicAPI]
public class ParameterState
{
    /// <summary>
    ///     Gets a value which represents the <see cref="MqttBindingSource"/> associated with the
    ///     request
    /// </summary>
    public MqttBindingSource? BindingSource { get; private set; }
    public bool IsModelSet { get; private set; }

    public object? Value { get; private set; }
    public ParameterInfo ParameterInfo { get; }
    public string ParameterName => ParameterInfo.Name!;
    public Type TargetType => ParameterInfo.ParameterType;

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Value" /> should be validated.
    /// </summary>
    public bool SuppressValidation => ValidationStatus == ParameterValidationResult.ValidationSuppressed;
    public ParameterValidationResult ValidationStatus { get; private set; } = ParameterValidationResult.NotYetValidated;
    public ValidationResult[]? ValidationResults { get; private set; }

    public IMqttModelBindingContext ModelBindingContext { get; }

    private bool _bindingAttempted;

    internal void SetBindingSuccess(MqttBindingSource bindingSource, object? value, bool bypassValidation = false)
    {
        if (_bindingAttempted)
        {
            throw new InvalidOperationException("Binding has already been attempted.");
        }

        if (ValidationResults != null)
        {
            throw new InvalidOperationException("Validation has already occurred.");
        }

        _bindingAttempted = true;

        BindingSource = bindingSource;
        Value = value;
        IsModelSet = true;

        if (bypassValidation)
        {
            ValidationStatus = ParameterValidationResult.ValidationSuppressed;
        }
    }

    internal void SetBindingFailure()
    {
        if (_bindingAttempted)
        {
            throw new InvalidOperationException("Binding has already been attempted.");
        }

        if (ValidationResults != null)
        {
            throw new InvalidOperationException("Validation has already occurred.");
        }

        _bindingAttempted = true;

        Value = null;
        IsModelSet = false;
    }

    internal void SetValidationResults(IEnumerable<ValidationResult> validationResults)
    {
        ArgumentNullException.ThrowIfNull(validationResults);

        ValidationResults = validationResults.ToArray();
        ValidationStatus = ValidationResults.Length == 0 ? ParameterValidationResult.Valid : ParameterValidationResult.Invalid;
    }

    public ParameterState(IMqttModelBindingContext modelBindingContext, ParameterInfo parameterInfo)
    {
        ModelBindingContext = modelBindingContext;
        ParameterInfo = parameterInfo;
    }

    public bool TryValidate()
    {
        // TODO
        ValidationStatus = ParameterValidationResult.Valid;
        return true;
    }

    private bool TryGetTypeConverter<TAttribute, TTypeConverter>(
        IMqttRequestContext requestContext,
        Func<TAttribute, Type?> typeConverterTypeSelector,
        [MaybeNullWhen(false)] out TTypeConverter typeConverter
    )
        where TAttribute : Attribute
        where TTypeConverter : class
    {
        var customAttribute = ParameterInfo.GetCustomAttributes().OfType<TAttribute>().SingleOrDefault();
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

        var serviceProvider = requestContext.ServiceProvider;

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
