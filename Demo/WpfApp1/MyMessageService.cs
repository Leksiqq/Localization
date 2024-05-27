using ConsoleApp1;
using Net.Leksi.Localization;

namespace WpfApp1;
[ResourcePlace("WpfApp1.Resources.Resource1")]
internal class MyMessageService: MessageService
{
    public string MainWindowTitle => GetString();
}
