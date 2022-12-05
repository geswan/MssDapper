using Dapper;
using System.Data;

namespace DataAccess
{
    public interface IDataAccess
    {
        string GetConnectionId();
        Task ExecuteTransactionAsync(string sqlA, object parametersA, string sqlB, object parametersB, string? connectionId=null  , CommandType? commandType=null );
        Task ExecuteAsync(string sql, object parameters, string? connectionId=null  , CommandType? commandType=null );
        Task<IEnumerable<T>> QueryAsync<T, S>(string sql, Func<T, S, T> mappingFunc, string? connectionId=null  , string splitOn = "Id", CommandType? commandType=null );
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters , string? connectionId=null  , CommandType? commandType=null );
        Task<T> QuerySingleAsync<T>(string sql, object? parameters , string? connectionId=null  , CommandType? commandType=null );
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters , string? connectionId=null  , CommandType? commandType=null );
        Task<dynamic> QueryAsyncDynamic(string sql, object? parameters, string? connectionId=null  , CommandType? commandType = null );
        Task BulkInsertAsync<T>(string table, IEnumerable<T> items, Dictionary<string, string>? mappingDic = null, string? connectionId = null);
    }

}