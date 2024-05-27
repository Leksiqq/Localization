using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using static System.Text.Encoding;

Console.OutputEncoding = Unicode;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<MessageService> ();
builder.Logging.SetMinimumLevel(LogLevel.Information);

using IHost host = builder.Build();

ILogger logger = host.Services.GetRequiredService<ILoggerFactory>()
        .CreateLogger("Localization.Example");

MessageService messageService = host.Services.GetRequiredService<MessageService>();

if (args is [var cultureName])
{
    messageService.Culture = CultureInfo.GetCultureInfo(cultureName);
}

logger.LogInformation("{Msg}", messageService.GreetingMessage);

logger.LogInformation("{Msg}", messageService.DinnerPriceFormat(DateTime.Today.AddDays(-3), 37.63));
