using System;
using Sholo.Mqtt.Old.Application;

namespace Sholo.Mqtt.Old.ApplicationProvider
{
    public interface IMqttApplicationProvider
    {
        event EventHandler<ApplicationChangedEventArgs> ApplicationChanged;

        void Rebuild();
        IMqttApplication Current { get; }
    }
}
