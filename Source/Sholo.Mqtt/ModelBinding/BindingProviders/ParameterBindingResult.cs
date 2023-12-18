namespace Sholo.Mqtt.ModelBinding.BindingProviders;

public class ParameterBindingResult
{
    public MqttBindingSource BindingSource { get; }
    public object? Value { get; }
    public bool BypassValidation { get; }

    public ParameterBindingResult(MqttBindingSource bindingSource, object? value, bool bypassValidation = false)
    {
        BindingSource = bindingSource;
        Value = value;
        BypassValidation = bypassValidation;
    }
}
