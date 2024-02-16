using Kata.Odata.DataModel.KataQuery.Models;
using SqlKata;

namespace Kata.Odata.DataModel.KataQuery
{
    public interface IODataQueryBuilder
    {
        Task<Query> CreateQuery<T>(Dictionary<string, string> options, string entityName) where T : class;
        Task<Query> CreateQuery<T>(Dictionary<string, string> options, string entityName, IEnumerable<int> aOrgs) where T : class;
        Task<SqlDapperQuery> GetSqlQuery(Query queryIn, bool bCount = false);
    }
}