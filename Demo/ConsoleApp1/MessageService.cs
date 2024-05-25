using Net.Leksi.Localization;

namespace ConsoleApp1;

internal class MessageService: LocalizationBase
{
    public string GreetingMessage => GetString();
    public string FormattedMessage => GetString();
}
