using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using MySqlConnector;
using System.Runtime.ExceptionServices;
using System.Transactions;
using System.Text;

namespace DataAccess;

public class MysqldataAccess : IDataAccess
{
    private readonly IConfiguration _config;
    private const string mySqlConnectionId = "MySql";//must be a const
    public MysqldataAccess(IConfiguration config)
    {
        _config = config;
    }

    public string GetConnectionId() => mySqlConnectionId;
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

    public async Task BulkInsertAsync(
        string tableName,
        Dictionary<string, object> paramDic, 
        IEnumerable<string> colNames,
        string? connectionId = null
        )
    {
        connectionId ??= mySqlConnectionId;
        int columnsPerRow = colNames.Count();
        StringBuilder sb = new("(@p0");
        for (int n = 1; n < paramDic.Count; n++)
        {
            string s = n % columnsPerRow == 0 ? $"),(@p{n}" : $",@p{n}";
           //builds the values in this form: (@p0,@p1,@p2),(@p3,@p4,@p5),....
            sb.Append(s);
        }
        sb.Append(')');
        string sql = $"INSERT INTO {tableName} ({string.Join(',', colNames)}) VALUES " + sb.ToString();

        using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionId));
        await connection.ExecuteAsync(sql, new DynamicParameters(paramDic));      
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
