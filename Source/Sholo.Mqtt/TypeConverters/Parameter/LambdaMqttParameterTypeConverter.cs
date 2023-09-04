using System;

namespace Sholo.Mqtt.TypeConverters.Parameter
{
    internal class LambdaMqttParameterTypeConverter<TTargetType> : IMqttParameterTypeConverter
    {
        public Func<string, TTargetType> Converter { get; }

        public LambdaMqttParameterTypeConverter(Func<string, TTargetType> converter)
        {
            Converter = converter;
        }

        public bool TryConvert(string value, Type targetType, out object result)
        {
            try
            {
                var typedResult = Converter.Invoke(value);
                result = typedResult;
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}
