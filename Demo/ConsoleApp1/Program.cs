using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using static System.Text.Encoding;


Console.OutputEncoding = Unicode;

if (args is [var cultureName])
{
    CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
}

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<MessageService> ();
builder.Logging.SetMinimumLevel(LogLevel.Information);

using IHost host = builder.Build();

ILogger logger = host.Services.GetRequiredService<ILoggerFactory>()
        .CreateLogger("Localization.Example");

MessageService messageService = host.Services.GetRequiredService<MessageService>();

logger.LogInformation("{Msg}", messageService.GreetingMessage);

logger.LogInformation("{Msg}", string.Format(messageService.FormattedMessage, DateTime.Today.AddDays(-3), 37.63));

foreach(CultureInfo culture in messageService.GetSupportedCultures())
{
    logger.LogInformation("{Msg}", culture);
}
