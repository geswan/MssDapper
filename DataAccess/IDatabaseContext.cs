using System.Data;

namespace DataAccess
{
    public interface IDatabaseContext
    {
        bool IsSqlServer{ get; }
        Task ExecuteTransactionAsync(string sqlA, object parametersA, string sqlB, object parametersB, CommandType? commandType=null );
        Task ExecuteAsync(string sql, object parameters,  CommandType? commandType=null );
        Task<IEnumerable<T>> QueryAsync<T, S>(string sql, Func<T, S, T> mappingFunc,  string splitOn = "Id", CommandType? commandType=null );
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters=null , CommandType? commandType=null );
        Task<T> QuerySingleAsync<T>(string sql, object? parameters ,  CommandType? commandType=null );
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters ,  CommandType? commandType=null );
        Task<dynamic> QueryAsyncDynamic(string sql, object? parameters=null,  CommandType? commandType = null );
        Task BulkInsertAsync<T>(IEnumerable<T> items, Dictionary<string, string>? mappingDic = null,  string? table=null );
    }

}