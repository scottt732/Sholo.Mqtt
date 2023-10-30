using System;
using System.Collections.Generic;
using System.Linq;
using Sholo.Mqtt.Application.Builder;
using Sholo.Mqtt.Application.BuilderConfiguration;

namespace Sholo.Mqtt.Application.Provider;

public class MqttApplicationProvider : IMqttApplicationProvider
{
    public IServiceProvider ServiceProvider { get; }
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

    public MqttApplicationProvider(
        IServiceProvider serviceProvider,
        IEnumerable<IConfigureMqttApplicationBuilder> configureMqttApplicationBuilders)
    {
        ServiceProvider = serviceProvider;
        ConfigureMqttApplicationBuilders = configureMqttApplicationBuilders?.ToArray() ?? Array.Empty<IConfigureMqttApplicationBuilder>();
    }

    public void Rebuild()
    {
        var applicationBuilder = new MqttApplicationBuilder(ServiceProvider);

        foreach (var configureMqttApplicationBuilder in ConfigureMqttApplicationBuilders)
        {
            configureMqttApplicationBuilder?.ConfigureMqttApplication(applicationBuilder);
        }

        Current = applicationBuilder.Build();
    }

    protected virtual void OnApplicationChanged(IMqttApplication previous, IMqttApplication current)
        => ApplicationChanged?.Invoke(this, new ApplicationChangedEventArgs(previous, current));
}
