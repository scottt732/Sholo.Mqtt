using System;
using System.Collections.Generic;
using System.Linq;
using Sholo.Mqtt.Old.Application;
using Sholo.Mqtt.Old.ApplicationBuilderConfiguration;

namespace Sholo.Mqtt.Old.ApplicationProvider
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
            /*
            var applicationBuilder = new MqttApplicationBuilder(null);

            foreach (var configureMqttApplicationBuilder in ConfigureMqttApplicationBuilders)
            {
                configureMqttApplicationBuilder?.ConfigureMqttApplication(applicationBuilder);
            }

            Current = applicationBuilder.Build();
            */
            throw new NotImplementedException();
        }

        protected virtual void OnApplicationChanged(IMqttApplication previous, IMqttApplication current)
            => ApplicationChanged?.Invoke(this, new ApplicationChangedEventArgs(previous, current));
    }
}
