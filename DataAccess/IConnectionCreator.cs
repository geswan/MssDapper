using System.Data;

namespace DataAccess
{
    public interface IConnectionCreator
    {
        IDbConnection CreateConnection();
    }
}