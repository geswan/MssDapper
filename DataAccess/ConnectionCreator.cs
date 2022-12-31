using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess
{
   //these are the factory classes for creating the required DatabaseConnection
    public class MySqlConnectionCreator : IConnectionCreator
   
    {
        protected ServerOptions _serverOptions;
        public MySqlConnectionCreator(IOptions<ServerOptions> serverOptionsSnapshot)
        {
            _serverOptions = serverOptionsSnapshot.Value;
        }

        public IDbConnection CreateConnection()

        {
            var connectionString = _serverOptions.MySql;
            return new MySqlConnection(connectionString);
        }
    }

    public class MsSqlConnectionCreator : IConnectionCreator
    {
        protected ServerOptions _serverOptions;
        public MsSqlConnectionCreator(IOptions<ServerOptions> serverOptionsSnapshot)
        {
            _serverOptions = serverOptionsSnapshot.Value;
        }
                                     
        public  IDbConnection CreateConnection()
        {
            var connectionString = _serverOptions.MsSql;
            return new SqlConnection(connectionString);
        }
    }
}
