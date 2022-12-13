﻿using Dapper;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Transactions;

namespace DataAccess;


public class MssDataAccessSql : IDataAccess
{
    private readonly IDatabaseContext _databaseContext;
 
    public MssDataAccessSql(IDatabaseContext databaseContext)
    {
        _databaseContext=databaseContext;
       IsSqlServer= _databaseContext.IsSqlServer;
    }

    public bool IsSqlServer {get; }
    public async Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CommandType? commandType = null)
    {
        //error here
        using IDbConnection connection = _databaseContext.GetConnection();
        var result = await connection.QueryAsync<T>(sql, parameters,
            commandType: commandType);

        return result;
    }

    public async Task<T> QuerySingleAsync<T>(
       string sql,
       object? parameters = null,
       CommandType? commandType = null)
    {
        using IDbConnection connection = _databaseContext.GetConnection();
        var result = await connection.QuerySingleAsync<T>(sql, parameters,
            commandType: commandType);

        return result;
    }

    public async Task<dynamic> QueryAsyncDynamic(
      string sql,
     object? parameters,
    CommandType? commandType = null)
    {
        using IDbConnection connection = _databaseContext.GetConnection();

        var result = await connection.QueryAsync<dynamic>(sql, parameters, commandType: commandType);

        return result;

    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(
       string sql,
       object? parameters = null,
       CommandType? commandType = null)
    {
        using IDbConnection connection = _databaseContext.GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters,
         commandType: commandType);
        return result;
    }


    public async Task<IEnumerable<T>> QueryAsync<T, S>(
          string sql,
          Func<T, S, T> mappingFunc,
         string splitOn = "Id",//dapper converts this to  uppercase, so ID,id is accepted
        CommandType? commandType = null)
    {
        using IDbConnection connection = _databaseContext.GetConnection();

        var results = await connection.QueryAsync(sql, mappingFunc, splitOn: splitOn, commandType: commandType);

        return results;

    }
    public async Task ExecuteAsync(
        string sql,
        object parameters,
        CommandType? commandType = null)
    {
        using IDbConnection connection = _databaseContext.GetConnection(); ;
        await connection.ExecuteAsync(sql, parameters, commandType: commandType);

    }


    public async Task BulkInsertAsync<T>(
     IEnumerable<T> items,
  Dictionary<string, string>? mappingDic = null,
   string? table = null
      )
    {
        using IDbConnection connection = _databaseContext.GetConnection();
        await connection.BulkInsertAsync(items, mappingDic!,table);

    }

    public async Task ExecuteTransactionAsync(
      string sqlA,
      object parametersA,
       string sqlB,
      object parametersB,
      CommandType? commandType = null)
    {
        using IDbConnection connection = _databaseContext.GetConnection();
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
