using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Localization;
using System.Globalization;

namespace ClassLibrary1;

public class Class2(IServiceProvider services)
{
    public void Test()
    {
        Localizer1 trans = services.GetRequiredService<Localizer1>();


        Console.WriteLine(CultureInfo.CurrentUICulture);
        Console.WriteLine(trans.String1);
        Console.WriteLine(trans.String2);
        Console.WriteLine(trans.String3);

        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = new CultureInfo("en-US");
        Console.WriteLine(CultureInfo.CurrentUICulture);
        Console.WriteLine(trans.String1);
        Console.WriteLine(trans.String2);
        Console.WriteLine(trans.String3);

        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = new CultureInfo("ru");
        Console.WriteLine(CultureInfo.CurrentUICulture);
        Console.WriteLine(trans.String1);
        Console.WriteLine(trans.String2);
        Console.WriteLine(trans.String3);

    }
}
