using ComplexFilters.Abstractions.Models;
using Kata.QueryBuilder.Models;
using SqlKata;

namespace Kata.QueryBuilder
{
    public interface IODataQueryParser
    {
        Task<ODataQueryResult> ExecuteQuery<T>(Dictionary<string, string> options, string schema, string tableName, IEnumerable<ComplexFilter>? defaultFilters = null) where T : class;
        Task<Query> CreateQuery<T>(Dictionary<string, string> options, string schema, string entityName, IEnumerable<ComplexFilter>? defaultFilters = default) where T : class;
        Task<ODataQueryResult> ExecQuery(Query queryIn);
        Task<SqlDapperQuery> GetSqlQuery(Query queryIn, bool bCount = false);
    }
}