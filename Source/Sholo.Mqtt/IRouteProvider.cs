namespace Sholo.Mqtt;

public interface IRouteProvider
{
    Endpoint[] Endpoints { get; }

    Endpoint? GetEndpoint(IMqttRequestContext context);
}
