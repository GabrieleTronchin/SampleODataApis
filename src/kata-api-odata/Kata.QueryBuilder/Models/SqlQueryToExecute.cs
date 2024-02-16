using Dapper;

namespace Kata.QueryBuilder.Models
{
    public class SqlQueryToExecute
    {
        public string TSqlQuery { get; set; }

        public bool IsCountQuery { get; set; }

        public IDictionary<string, object> Parameters { get; set; }
    }


    public class SqlDapperQuery
    {
        public string SqlCommand { get; set; }

        public DynamicParameters Parameters { get; set; }
    }

}
