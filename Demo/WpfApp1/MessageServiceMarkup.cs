using System.Windows;
using System.Windows.Markup;

namespace WpfApp1;
[MarkupExtensionReturnType(typeof(MyMessageService))]
internal class MessageServiceMarkup : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Application.Current.Resources["MessageService"];
    }
}
