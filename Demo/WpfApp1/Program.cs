using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WpfApp1;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton<MyMessageService>();
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<MainWindow>();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        using IHost host = builder.Build();

        App app = host.Services.GetRequiredService<App>();
        app.Resources["MessageService"] = host.Services.GetRequiredService<MyMessageService>();
        app.Run(host.Services.GetRequiredService<MainWindow>());
    }
}
