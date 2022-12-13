using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MssDapper
{
    public class Startup
    {
        private IConfiguration _config;
       
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataAccess, MssDataAccessSql>();
            // services.AddTransient<IDatabaseContext, SqlServerContext>();
            //use this for MySql and MariaDB
            services.AddTransient<IDatabaseContext, MySqlServerContext>();
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
            services.Configure<ServerOptions>(_config.GetSection(ServerOptions.ConnectionStrings));
          
        }
    }
}
