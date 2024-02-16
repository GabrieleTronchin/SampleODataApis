

using SqlKata;

namespace Kata.QueryBuilder.Models
{
    public class QueryInfo
    {
        public Query Query { get; set; } //oggetto query 
        public string EntityName { get; set; } //nome entità principale su cui viene eseguita la query 
    }
}
