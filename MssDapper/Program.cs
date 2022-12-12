using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MssDapper;

var host = CreateApplicationBuilder().Build();
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider svProvider = serviceScope.ServiceProvider;//use container for this scope
//use dependency injection where possible rather than GetService
var loggerFactory = svProvider.GetService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("built host");
var demo = svProvider.GetService<Demo>();
 await demo.Run();
Console.WriteLine();


static HostApplicationBuilder CreateApplicationBuilder()
{
    var builder = Host.CreateApplicationBuilder();
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);
    return builder;
}