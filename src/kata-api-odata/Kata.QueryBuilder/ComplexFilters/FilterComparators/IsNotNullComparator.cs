using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;

namespace Kata.QueryBuilder.ComplexFilters.FilterComparators
{
    public class IsNotNullComparator : IComparator
    {
        public IsNotNullComparator()
        {
        }

        public FilterBuilder ComposeExpression(FilterBuilder filterBuilder, Filter filter)
        {
            filterBuilder.Expression.Append($"{filter.FilterColumn}.HasValue");
            return filterBuilder;
        }

        public FilterBuilder ComposeExpression(FilterBuilder filterBuilder, Filter filter, string columnName)
        {
            filterBuilder.Expression.Append($"{columnName}.HasValue");
            return filterBuilder;
        }
    }
}
