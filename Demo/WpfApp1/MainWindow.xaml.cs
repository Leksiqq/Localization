using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
namespace WpfApp1;
public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<CultureInfo> Cultures { get; private init; } = [];
    public string DinnerPriceFormat => (Application.Current.Resources["MessageService"] as MyMessageService)!.DinnerPriceFormat(DateTime.Today.AddDays(-3), 37.63);
    public MainWindow(IServiceProvider services)
    {
        MyMessageService messageService = (Application.Current.Resources["MessageService"] as MyMessageService)!;
        messageService.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, e);
        Cultures.Add(messageService.Culture!);
        Cultures.Add(CultureInfo.GetCultureInfo("en-US"));
        Cultures.Add(CultureInfo.GetCultureInfo("sr-Latn"));
        Cultures.Add(CultureInfo.GetCultureInfo("sr-Latn-RS"));
        Cultures.Add(CultureInfo.GetCultureInfo("sr-Cyrl"));
        Cultures.Add(CultureInfo.GetCultureInfo("sr-Cyrl-RS"));
        Cultures.Add(CultureInfo.GetCultureInfo("fr"));
        Cultures.Add(CultureInfo.GetCultureInfo("fr-CA"));
        InitializeComponent();
        CulturesList.SelectedItem = messageService.Culture!;
    }
    private void CulturesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        MyMessageService messageService = (Application.Current.Resources["MessageService"] as MyMessageService)!;
        messageService.Culture = CulturesList.SelectedItem as CultureInfo;
    }
}
