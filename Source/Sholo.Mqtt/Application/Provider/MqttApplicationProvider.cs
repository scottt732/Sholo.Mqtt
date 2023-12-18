using System;
using System.Collections.Generic;
using System.Linq;
using Sholo.Mqtt.Application.Builder;
using Sholo.Mqtt.Application.BuilderConfiguration;

namespace Sholo.Mqtt.Application.Provider;

public sealed class MqttApplicationProvider : IMqttApplicationProvider
{
    private IServiceProvider ServiceProvider { get; }

    public event EventHandler<ApplicationChangedEventArgs>? ApplicationChanged;

    public IMqttApplication? Current
    {
        get => _current;
        private set
        {
            var previous = _current;
            _current = value;

            OnApplicationChanged(previous, _current!);
        }
    }

    private IMqttApplication? _current;
    private IConfigureMqttApplicationBuilder[] ConfigureMqttApplicationBuilders { get; }

    public MqttApplicationProvider(
        IServiceProvider serviceProvider,
        IEnumerable<IConfigureMqttApplicationBuilder> configureMqttApplicationBuilders)
    {
        ServiceProvider = serviceProvider;
        ConfigureMqttApplicationBuilders = configureMqttApplicationBuilders.ToArray();
    }

    public void Rebuild()
    {
        var applicationBuilder = new MqttApplicationBuilder(ServiceProvider);

        foreach (var configureMqttApplicationBuilder in ConfigureMqttApplicationBuilders)
        {
            configureMqttApplicationBuilder.ConfigureMqttApplication(applicationBuilder);
        }

        Current = applicationBuilder.Build();
    }

    private void OnApplicationChanged(IMqttApplication? previous, IMqttApplication current)
        => ApplicationChanged?.Invoke(this, new ApplicationChangedEventArgs(previous, current));
}
