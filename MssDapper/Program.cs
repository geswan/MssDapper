using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MssDapper;


/***Default settings for Application Builder
  Load app IConfiguration from 'appsettings.json' 
Load app IConfiguration from User Secrets when EnvironmentName is 'Development'
Load app IConfiguration from environment variables.
Configure the ILoggerFactory to log to the console, debug, and event source output.
 ***/


var builder = Host.CreateApplicationBuilder();
#region Configuration

var section = builder.Configuration.GetSection(ServerOptions.ConnectionStrings);
builder.Services.Configure<ServerOptions>(section);
#endregion

#region Add services
builder.Services.AddScoped<IDataAccess, MssDataAccessSql>()
               //Add SqlServerContext for MicrosoftSqlServer
               //or add  MySqlServerContext for MySql or MariaDb
               // AddTransient<IDatabaseContext, SqlServerContext>()
               .AddTransient<IDatabaseContext, MySqlServerContext>()
               .AddScoped<SpExampleIds>()
               .AddScoped<Helper>()
               .AddScoped<Examples>()
               .AddScoped<TransactionExample>()
               .AddScoped<Demo>();
#endregion
var app = builder.Build();
var logger = app.Services.GetService<ILogger<Program>>();
logger?.LogInformation("built app");
var demo = app.Services.GetService<Demo>();
if (demo != null)
{
    await demo.Run();
}


