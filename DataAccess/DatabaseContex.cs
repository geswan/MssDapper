using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess;

public class SqlServerContext : IDatabaseContext
{
    private readonly ServerOptions _serverOptions;
    private readonly string _connectionString;
    public bool IsSqlServer { get; } = true;
  //the snapshot is of the connection strings in appsettings.json
  //at the time when the class was instantiated.The class was added to the container 
  //as a transient
    public SqlServerContext(IOptionsSnapshot<ServerOptions> serverOptionsSnapshot)
    {
        _serverOptions = serverOptionsSnapshot.Value;
        _connectionString = _serverOptions.MsSql;
    
    }
    public IDbConnection GetConnection()
    {

        return new SqlConnection(_connectionString);
    }
}

public class MySqlServerContext : IDatabaseContext
{
  
    private readonly ServerOptions _serverOptions;
    private readonly string _connectionString;
    public bool IsSqlServer { get; } = false;
    public MySqlServerContext(IOptionsSnapshot<ServerOptions> serverOptionsSnapshot)
    {
      
        _serverOptions = serverOptionsSnapshot.Value;
        _connectionString = _serverOptions.MySql;
    }
    public IDbConnection GetConnection()
    {
       
        return new MySqlConnection(_connectionString);
    }
  
}
