using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MssDapper;

var builder = CreateApplicationBuilder();
builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
builder.Logging.AddDebug();
var app= builder.Build();
var logger=app.Services.GetService<ILogger<Program>>();
logger?.LogInformation("built app");
var demo = app.Services.GetService<Demo>();
 await demo.Run();
Console.WriteLine();


static HostApplicationBuilder CreateApplicationBuilder()
{
    var builder = Host.CreateApplicationBuilder();
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);
    return builder;
}