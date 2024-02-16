

namespace Kata.QueryBuilder.Models
{
    public class WhereFilter
    {
        public WhereFilter(string field, object value)
        {
            Field = field;
            Value = value;
        }

        public string Field { get; set; }
        public object Value { get; set; }
    }
}
