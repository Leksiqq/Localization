﻿namespace Net.Leksi.Localization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ResourcePlaceAttribute(string baseName): Attribute
{
    public string BaseName { get; private init; } = baseName;
    public string? OtherAssemblyFullName { get; init; }
}
