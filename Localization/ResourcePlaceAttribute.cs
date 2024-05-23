namespace Net.Leksi.Localization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ResourcePlaceAttribute(string baseName): Attribute
{
    public string BaseName { get; private init; } = baseName;
    public Type? TargetType { get; init; }
    public ResourceAssembly? ResourceAssembly { get; init; }
    public string? OtherAssemblyFullName { get; init; }
}
