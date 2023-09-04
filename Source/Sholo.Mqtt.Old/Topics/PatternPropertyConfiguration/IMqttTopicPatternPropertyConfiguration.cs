namespace Sholo.Mqtt.Old.Topics.PatternPropertyConfiguration
{
    public interface IMqttTopicPatternPropertyConfiguration<in TTopicParameters>
    {
        string ParameterName { get; }
        bool HaveTypeConverter { get; }
        void SetValue(TTopicParameters target, string value);
    }
}
