using System;
using System.Collections.Generic;
using System.Linq;
using Sholo.Mqtt.Application;
using Sholo.Mqtt.ApplicationBuilder;
using Sholo.Mqtt.ApplicationBuilderConfiguration;

namespace Sholo.Mqtt.ApplicationProvider
{
    public class MqttApplicationProvider : IMqttApplicationProvider
    {
        public event EventHandler<ApplicationChangedEventArgs> ApplicationChanged;

        public IMqttApplication Current
        {
            get => _current;
            set
            {
                var previous = _current;
                _current = value;

                OnApplicationChanged(previous, _current);
            }
        }

        private IMqttApplication _current;
        private IConfigureMqttApplicationBuilder[] ConfigureMqttApplicationBuilders { get; }

        public MqttApplicationProvider(IEnumerable<IConfigureMqttApplicationBuilder> configureMqttApplicationBuilders)
        {
            ConfigureMqttApplicationBuilders = configureMqttApplicationBuilders?.ToArray() ?? Array.Empty<IConfigureMqttApplicationBuilder>();
        }

        public void Rebuild()
        {
            var applicationBuilder = new MqttApplicationBuilder();

            foreach (var configureMqttApplicationBuilder in ConfigureMqttApplicationBuilders)
            {
                configureMqttApplicationBuilder?.ConfigureMqttApplication(applicationBuilder);
            }

            Current = applicationBuilder.Build();
        }

        protected virtual void OnApplicationChanged(IMqttApplication previous, IMqttApplication current)
            => ApplicationChanged?.Invoke(this, new ApplicationChangedEventArgs(previous, current));
    }
}
