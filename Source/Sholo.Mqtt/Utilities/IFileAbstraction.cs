namespace Sholo.Mqtt.Utilities;

[PublicAPI]
public interface IFileAbstraction
{
    bool Exists(string? path);
}
