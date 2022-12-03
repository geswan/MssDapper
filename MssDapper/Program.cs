
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MssDapper;

using System.Text;

var host = CreateDefaultBuilder().Build();
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider svProvider = serviceScope.ServiceProvider;//use container for this scope
//use dependency injection rather than GetService
var loggerFactory = svProvider.GetService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("built host");
var demo = svProvider.GetService<Demo>();
 await demo.Run();
Console.WriteLine();


static IHostBuilder CreateDefaultBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(app =>
        {
            app.AddJsonFile("appsettings.json");//set file to always copy to exe location
        })
        .ConfigureServices(services =>
        {
            services.AddTransient<StringBuilder>();
           services.AddSingleton<IDataAccess,MssDataAccessSql>();
           // services.AddSingleton<IDataAccess, MysqldataAccess>();
            services.AddTransient<SpExampleIds>();
            services.AddTransient<Helper>();
            services.AddTransient<Examples>();
            services.AddTransient<TransactionExample>();
            services.AddTransient<Demo>();
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                //  builder.AddConsole();
                builder.AddDebug();//output path is View/Output/Debug
            });
        });
}