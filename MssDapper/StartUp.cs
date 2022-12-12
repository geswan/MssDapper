using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MssDapper
{
    public class Startup
    {
        public IConfiguration Config
        {
            get;
        }
        public Startup(IConfiguration configuration)
        {
            Config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataAccess, MssDataAccessSql>();
            // services.AddScoped<IDatabaseContext, SqlServerContext>();
            //use this for MySql and MariaDB
            services.AddScoped<IDatabaseContext, MySqlServerContext>();
            services.AddScoped<SpExampleIds>();
            services.AddScoped<Helper>();
            services.AddScoped<Examples>();
            services.AddScoped<TransactionExample>();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                //  loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();//output path is View/Output/Debug
            });
            services.AddScoped<Demo>();
            services.Configure<ServerOptions>(Config.GetSection(ServerOptions.Servers));
          
        }
    }
}
