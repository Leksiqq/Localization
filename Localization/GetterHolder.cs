namespace Net.Leksi.Localization;

internal class GetterHolder
{
    internal Type ReturnType { get; set; } = null!;
    internal string? BaseName { get; set; }
    internal Func<string, object?> Getter { get; set; } = null!;
}
