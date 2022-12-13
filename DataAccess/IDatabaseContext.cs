using System.Data;

namespace DataAccess
{
    public interface IDatabaseContext
    {
        IDbConnection GetConnection();
        bool IsSqlServer { get; }
    }
}