using ClassLibrary1;
using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Localization;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<Localizer1, Class1>();
builder.Services.AddTransient<Class2>();
var app = builder.Build();
app.Services.GetRequiredService<Class2>().Test();

