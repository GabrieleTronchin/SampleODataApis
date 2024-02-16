using Kata.Odata.DataModel.KataQuery.Models;
using SqlKata;

namespace Kata.Odata.DataModel.KataQuery.QueryParser
{
    public interface IODataQueryParser
    {
        Task<ODataQueryResult> ExecuteQueryAsync<T>(Dictionary<string, string> options, string schema, string tableName) where T : class;
        Task<Query> CreateQueryAsync<T>(Dictionary<string, string> options, string schema, string entityName) where T : class;
        Task<ODataQueryResult> ExecuteQueryAsync(Query queryIn);
        Task<SqlQuery> GenerateQuery(Query queryIn, bool bCount = false);
    }
}