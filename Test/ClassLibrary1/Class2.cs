using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Localization;
using System.Globalization;

namespace ClassLibrary1;

public class Class2(IServiceProvider services)
{
    public void Test()
    {
        Localizer1 trans = services.GetRequiredService<Localizer1>();
        Console.WriteLine(trans.Culture);
        Console.WriteLine(trans.String1);
        Console.WriteLine(trans.String2);
        Console.WriteLine(trans.String3);
        Console.WriteLine();
        trans.Culture = new CultureInfo("en-US");
        Console.WriteLine(trans.Culture);
        Console.WriteLine(trans.String1);
        Console.WriteLine(trans.String2);
        Console.WriteLine(trans.String3);
        Console.WriteLine();
        trans.Culture = new CultureInfo("ru");
        Console.WriteLine(trans.Culture);
        Console.WriteLine(trans.String1);
        Console.WriteLine(trans.String2);
        Console.WriteLine(trans.String3);

    }
}
