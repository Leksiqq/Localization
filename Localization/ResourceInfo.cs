using System.Globalization;

namespace Net.Leksi.Localization;

public class ResourceInfo
{
    public string Name { get; internal set; } = null!;
    public Type DeclaringType { get; internal set; } = null!;
    public Type ReturnType { get; internal set; } = null!;
    public object? Value { get; internal set; }
    public string? BaseName { get; internal set; }
    public CultureInfo? Culture { get; internal set; }
}
