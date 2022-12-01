using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using MySqlConnector;
using System.Runtime.ExceptionServices;
using System.Transactions;


namespace DataAccess;

public class MysqldataAccess : IDataAccess
{
    private readonly IConfiguration _config;
    private const string mySqlConnectionId = "MySql";//must be a const
    public MysqldataAccess(IConfiguration config)
    {
        _config = config;
    }
    public async Task<IEnumerable<T>> QueryAsync<T>(
          string sql,
          object? parameters = null,
          string? connectionId = null,
          CommandType? commandType = null)
    {

        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));
        var result = await connection.QueryAsync<T>(sql, parameters,
            commandType: commandType);

        return result;
    }
    public async Task<T> QuerySingleAsync<T>(
       string sql,
       object? parameters = null,
        string? connectionId = null,
       CommandType? commandType = null)
    {
        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));
        var result = await connection.QuerySingleAsync<T>(sql, parameters,
            commandType: commandType);

        return result;
    }

    public async Task<int> BulkInsertAsync(
     string sql,
    IEnumerable<DynamicParameters> parameters,
     string? connectionId = null
    )
    {
        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));
        int totalInserts = 0;
        foreach (var p in parameters)
        {
            int result = await connection.ExecuteAsync(sql, p,commandType: CommandType.StoredProcedure);
            totalInserts += result;
        }
        return totalInserts;
    }


    public async Task<T> QueryFirstOrDefaultAsync<T>(
       string sql,
       object? parameters = null,
       string? connectionId = null,
       CommandType? commandType = null)
    {
        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));
        var result = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters,
         commandType: commandType);
        return result;
    }


    public async Task<IEnumerable<T>> QueryAsync<T, S>(
          string sql,
          Func<T, S, T> mappingFunc,
          string? connectionId = null,
          string splitOn = "Id",//default is where splitOn.ToUpper()==ID
         CommandType? commandType = null)
    {
        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));

        var results = await connection.QueryAsync(sql, mappingFunc, splitOn: splitOn, commandType: commandType);

        return results;

    }

    public async Task<dynamic> QueryAsyncDynamic(
        string sql,
        object? parameters,
        string? connectionId = null,
        CommandType? commandType = null)
    {
        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));

        var result = await connection.QueryAsync<dynamic>(sql, parameters, commandType: commandType);

        return result;

    }
    public async Task ExecuteAsync(
        string sql,
        object parameters,
        string? connectionId = null,
        CommandType? commandType = null)
    {
        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId)); ;
        await connection.ExecuteAsync(sql, parameters,
           commandType: commandType);

    }

    public async Task ExecuteTransactionAsync(
      string sqlA,
      object parametersA,
      string sqlB,
      object parametersB,
      string? connectionId = null,
      CommandType? commandType = null)
    {
        connectionId ??= mySqlConnectionId;
        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));
        using var transScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        connection.Open();

        try
        {
            int rowsAffectedA = await connection.ExecuteAsync(sqlA, parametersA);
            int rowsAffectedB = await connection.ExecuteAsync(sqlB, parametersB);

            if (rowsAffectedA == 0 || rowsAffectedB == 0)
            {
                throw new InvalidOperationException($"One or more operations failed");
            }

        }
        catch (Exception ex)
        {
            //preserves the stack on rethrow
            var exceptionInfo = ExceptionDispatchInfo.Capture(ex);
            exceptionInfo.Throw();

        }
        transScope.Complete();
    }
}
