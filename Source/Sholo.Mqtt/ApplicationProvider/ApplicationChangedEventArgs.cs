using System;
using Sholo.Mqtt.Application;

namespace Sholo.Mqtt.ApplicationProvider
{
    public class ApplicationChangedEventArgs : EventArgs
    {
        public IMqttApplication Previous { get; }
        public IMqttApplication Current { get; }

        public ApplicationChangedEventArgs(IMqttApplication previous, IMqttApplication current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
