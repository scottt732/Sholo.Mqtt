using System;

namespace Sholo.Mqtt.Application.Provider;

public class ApplicationChangedEventArgs : EventArgs
{
    public IMqttApplication? Previous { get; }
    public IMqttApplication Current { get; }

    public ApplicationChangedEventArgs(IMqttApplication? previous, IMqttApplication current)
    {
        Previous = previous;
        Current = current;
    }
}
