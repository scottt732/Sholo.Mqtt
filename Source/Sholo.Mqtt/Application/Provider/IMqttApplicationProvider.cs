using System;

namespace Sholo.Mqtt.Application.Provider
{
    public interface IMqttApplicationProvider
    {
        event EventHandler<ApplicationChangedEventArgs> ApplicationChanged;

        void Rebuild();
        IMqttApplication Current { get; }
    }
}
