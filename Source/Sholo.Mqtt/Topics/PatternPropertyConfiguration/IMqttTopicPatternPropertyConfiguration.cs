namespace Sholo.Mqtt.Topics.PatternPropertyConfiguration
{
    public interface IMqttTopicPatternPropertyConfiguration
    {
        string ParameterName { get; }
        bool HaveTypeConverter { get; }
        object GetParameterValue(string value);
    }
}
