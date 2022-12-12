using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess
{
    public class SqlServerContext : IDatabaseContext
    {
        private readonly IConfiguration _config;
        private const string _msConnectionId = "MsSql";

        public string ConnectionID
        {
            get { return _msConnectionId; }
        }
        public SqlServerContext(IConfiguration config)
        {
            _config = config;
        }
        public IDbConnection GetConnection(string? connectionId = null)
        {
            connectionId ??= _msConnectionId;
            return new SqlConnection(_config.GetConnectionString(connectionId));
        }
    }

    public class MySqlServerContext : IDatabaseContext
    {
        private readonly IConfiguration _config;
        private const string mySqlConnectionId = "MySql";

        public string ConnectionID
        {
            get { return mySqlConnectionId; }
        }
        public MySqlServerContext(IConfiguration config)
        {
            _config = config;
        }
        public IDbConnection GetConnection(string? connectionId = null)
        {
            connectionId ??= mySqlConnectionId;
            return new MySqlConnection(_config.GetConnectionString(connectionId));
        }
    }
}
