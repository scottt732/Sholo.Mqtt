using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Sholo.Mqtt.Utilities;

[ExcludeFromCodeCoverage]
internal class FileAbstraction : IFileAbstraction
{
    public bool Exists(string? path) => File.Exists(path);
}
