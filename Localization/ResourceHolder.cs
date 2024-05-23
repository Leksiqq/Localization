using System.Reflection;
using System.Resources;

namespace Net.Leksi.Localization;

internal class ResourceHolder
{
    internal Assembly Assembly { get; set; } = null!;
    internal string BaseName { get; set; } = null!;
    internal ResourceManager ResourceManager { get; set; } = null!;
}
