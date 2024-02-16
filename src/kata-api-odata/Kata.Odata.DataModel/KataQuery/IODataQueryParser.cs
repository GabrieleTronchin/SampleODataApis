using Kata.Odata.DataModel.KataQuery.Models;
using SqlKata;

namespace Kata.Odata.DataModel.KataQuery
{
    public interface IODataQueryParser
    {
        Task<ODataQueryResult> ExecuteQuery<T>(Dictionary<string, string> options, string schema, string tableName) where T : class;
        Task<Query> CreateQuery<T>(Dictionary<string, string> options, string schema, string entityName) where T : class;
        Task<ODataQueryResult> ExecQuery(Query queryIn);
        Task<SqlDapperQuery> GetSqlQuery(Query queryIn, bool bCount = false);
    }
}