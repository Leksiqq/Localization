using System.Globalization;

namespace Net.Leksi.Localization;

internal class ValueHolder
{
    internal Type ReturnType { get; set; } = null!;
    internal string? BaseName { get; set; }
    internal object? Value { get; set; }
    internal CultureInfo? Culture { get; set; }
}
