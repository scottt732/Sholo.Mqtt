using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sholo.Mqtt.Internal;

namespace Sholo.Mqtt.Application.Builder;

public class MqttApplicationBuilder : IMqttApplicationBuilder
{
    public IServiceProvider ApplicationServices { get; set; }
    public IDictionary<string, object> Properties { get; }

    private List<Func<MqttRequestDelegate, MqttRequestDelegate>> Middleware { get; }

    public MqttApplicationBuilder(IServiceProvider serviceProvider)
        : this()
    {
        Properties = new Dictionary<string, object>(StringComparer.Ordinal);
        ApplicationServices = serviceProvider;
    }

    public MqttApplicationBuilder(MqttApplicationBuilder builder)
        : this()
    {
        Properties = new CopyOnWriteDictionary<string, object>(builder.Properties, StringComparer.Ordinal);
    }

    private MqttApplicationBuilder()
    {
        Middleware = new List<Func<MqttRequestDelegate, MqttRequestDelegate>>();
    }

    public IMqttApplicationBuilder Use(Func<MqttRequestDelegate, MqttRequestDelegate> middleware)
    {
        Middleware.Add(middleware);
        return this;
    }

    public IMqttApplicationBuilder New()
    {
        return new MqttApplicationBuilder(this);
    }

    public IMqttApplication Build()
    {
        MqttRequestDelegate app = _ => Task.FromResult(false);

        for (var c = Middleware.Count - 1; c >= 0; c--)
        {
            app = Middleware[c](app);
        }

        var routeProvider = ApplicationServices.GetRequiredService<IRouteProvider>();

        var topicFilters = routeProvider.Endpoints?.Select(x => x.TopicPatternFilter.TopicFilter).ToArray();

        return new MqttApplication(topicFilters, app);
    }
}
