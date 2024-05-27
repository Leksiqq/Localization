using Net.Leksi.Localization;

namespace ConsoleApp1;
[ResourcePlace("ConsoleApp1.Resources.Messages")]
public class MessageService: LocalizationBase
{
    public string GreetingMessage => GetString();
    public string DinnerPriceFormat(params object?[] args) => GetFormattedString(args);
}
