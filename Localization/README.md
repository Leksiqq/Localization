The **Net.Leksi.Localization** library is designed to simplify the process of localizing and internationalizing applications. It is a wrapper for the standard application resource system, allowing you to separate localization actions in the code from actions with resources, increasing the clarity and readability of the code.

All classes are contained in the *Net.Leksi.Localization* namespace.

* [LocalizationBase](https://github.com/Leksiqq/Localization/wiki/LocalizationBase-en) - base class for objects whose properties contain values that depend on a given culture.
* [ResourcePlaceAttribute](https://github.com/Leksiqq/Localization/wiki/ResourcePlaceAttribute-en) is an attribute of a class derived from [LocalizationBase](LocalizationBase-en) that associates it with an application or other assembly resource that contains culture-specific values.
* [ResourceInfo](https://github.com/Leksiqq/Localization/wiki/ResourceInfo-en) - auxiliary class for use in the development process. Contains information about a culture-specific value: key, value, value type, what class it is a property of, the name of the resource it came from, the *locale* of that resource.

The principles of using the library are described in the section [Demo and principles of use] using the example of a test application from the article [Localization in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/localization) and demo applications based on WPF.

Sources are [here](https://github.com/Leksiqq/Localization/tree/master)

NuGet Package: [Net.Leksi.Localization](https://www.nuget.org/packages/Net.Leksi.Localization/)

