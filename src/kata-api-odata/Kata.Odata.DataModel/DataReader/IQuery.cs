using Dapper;

namespace Kata.Odata.DataModel.DataReader
{
    public interface IQuery
    {
        Task<IEnumerable<dynamic>> QueryAsync(string sqlCommand, DynamicParameters? parameter = null);

        Task<int> CountAsync(string sqlCommand, DynamicParameters? parameters = null);
    }
}
