using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MssDapper;

var builder = CreateApplicationBuilder();
var app= builder.Build();
var logger=app.Services.GetService<ILogger<Program>>();
logger?.LogInformation("built app");
var demo = app.Services.GetService<Demo>();
if (demo != null)
{
    await demo.Run();
}

Console.WriteLine();


static HostApplicationBuilder CreateApplicationBuilder()
{
    var builder = Host.CreateApplicationBuilder();
    var startup = new Startup(builder.Configuration);
    startup.Configure(builder);
    startup.ConfigureServices(builder.Services);
    return builder;
}