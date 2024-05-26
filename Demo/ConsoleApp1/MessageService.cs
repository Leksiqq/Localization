using Net.Leksi.Localization;

namespace ConsoleApp1;
[ResourcePlace("ConsoleApp1.Resources.Messages")]
internal class MessageService: LocalizationBase
{
    public string GreetingMessage => GetString();
    public string FormattedMessage => GetString();
}
