using System.Globalization;

namespace Net.Leksi.Localization;

public class ResourceInfo
{
    public string Name { get; set; } = null!;
    public Type DeclaringType { get; set; } = null!;
    public Type ReturnType { get; set; } = null!;
    public object? Value { get; set; }
    public string? BaseName { get; set; }
    internal CultureInfo? Culture { get; set; }
}
