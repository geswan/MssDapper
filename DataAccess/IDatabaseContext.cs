using System.Data;

namespace DataAccess
{
    public interface IDatabaseContext
    {
        IDbConnection GetConnection(string? connectionId = null);
        string ConnectionID { get; }
    }
}