using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;

namespace Kata.QueryBuilder.ComplexFilters.FilterComparators
{
    public class EndWithComparator : IComparator
    {
        public EndWithComparator()
        {
        }

        public FilterBuilder ComposeExpression(FilterBuilder filterBuilder, Filter filter)
        {
            filterBuilder.Expression.Append($"{filter.FilterColumn}.EndsWith(@{filterBuilder.ExpressionValues.Count})");
            filterBuilder.ExpressionValues.Add(filter.FilterValue.SingleOrDefault()?.ToString() ?? throw new NullReferenceException("Invalid Filter"));
            return filterBuilder;
        }

        public FilterBuilder ComposeExpression(FilterBuilder filterBuilder, Filter filter, string columnName)
        {
            filterBuilder.Expression.Append($"{columnName}.EndsWith(@{filterBuilder.ExpressionValues.Count})");
            filterBuilder.ExpressionValues.Add(filter.FilterValue.SingleOrDefault()?.ToString() ?? throw new NullReferenceException("Invalid Filter"));
            return filterBuilder;
        }
    }
}
