using System.Data;

namespace Kata.Odata.DataModel.DataReader
{
    public interface IDataReader : IQuery
    {
        IDbConnection CreateConnection();
        IDataReader GetContext(string connectionString);
    }
}
