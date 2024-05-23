namespace Net.Leksi.Localization;

[ResourcePlace("ClassLibrary1.Resource1")]
public class Localizer1: Core
{
    public virtual string String1 => GetString();
    public virtual string String2 => GetString();
    public virtual string String3 => GetString();
}
