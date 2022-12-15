using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MssDapper
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;   
        }

        public void Configure(HostApplicationBuilder builder)
        {
            builder.Configuration.AddUserSecrets<Program>();
            builder.Logging.ClearProviders();
            //builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Services.Configure<ServerOptions>(_config.GetSection(ServerOptions.ConnectionStrings));
    
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataAccess, MssDataAccessSql>();
          services.AddTransient<IDatabaseContext, SqlServerContext>();
            //use this for MySql and MariaDB
            //services.AddTransient<IDatabaseContext, MySqlServerContext>();
            services.AddScoped<SpExampleIds>();
            services.AddScoped<Helper>();
            services.AddScoped<Examples>();
            services.AddScoped<TransactionExample>();
            services.AddScoped<Demo>();
         }
    }
}
