using SqlKata;

namespace Kata.Odata.DataModel.KataQuery.Models
{
    public class QueryInfo
    {
        public Query? Query { get; set; }
        public required string EntityName { get; set; }
    }
}
