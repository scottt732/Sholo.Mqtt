using System;
using Sholo.Mqtt.Application;

namespace Sholo.Mqtt.ApplicationProvider
{
    public interface IMqttApplicationProvider
    {
        event EventHandler<ApplicationChangedEventArgs> ApplicationChanged;

        void Rebuild();
        IMqttApplication Current { get; }
    }
}
