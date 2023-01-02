using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MssDapper;


/***Default settings for HostApplicationBuilder:
Loads app IConfiguration from 'appsettings.json' 
Loads app IConfiguration from User Secrets when EnvironmentName is 'Development'
Loads app IConfiguration from environment variables.
Configures the ILoggerFactory to log to the console, debug, and event source output.
 ***/


var builder = Host.CreateApplicationBuilder();
#region Configuration
//bind appsettings ConnectionStrings section members MsSql,MySql to
//ServerOptions properties MsSql,MySql
var section = builder.Configuration.GetSection(ServerOptions.ConnectionStrings);
builder.Services.Configure<ServerOptions>(section);
#endregion

#region Add services
builder.Services.AddScoped<IDatabaseContext, DatabaseContext>()
               //Add  MsSqlConnectionCreator for MicrosoftSqlServer
               //or add  MySqlConnectionCreator for MySql or MariaDb
               .AddScoped<IConnectionCreator, MsSqlConnectionCreator>()
               //.AddScoped<IConnectionCreator, MySqlConnectionCreator>()
               .AddScoped<StoredProcedureId>()
               .AddScoped<Helper>()
               .AddScoped<Examples>()
               .AddScoped<TransactionExample>()
               .AddScoped<DemoA>();
#endregion
var app = builder.Build();
var demo = app.Services.GetService<DemoA>();
if (demo != null)
{
    await demo.Run();
}


