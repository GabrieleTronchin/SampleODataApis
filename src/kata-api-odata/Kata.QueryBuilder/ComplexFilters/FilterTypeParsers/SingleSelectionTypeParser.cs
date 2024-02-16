using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;

namespace Kata.QueryBuilder.ComplexFilters.FilterTypeParsers
{
    public class SingleSelectionTypeParser : IFilterTypeParser
    {


        public FilterBuilder Parse(FilterBuilder filterBuilder, Filter filter)
        {
            if (!filter.FilterValue.Any()) throw new InvalidOperationException($"Invalid Filter input parameter. {nameof(SingleSelectionTypeParser)} filter admit only 1 value.");
            filterBuilder.Expression.Append($"{filter.FilterColumn} = @{filterBuilder.ExpressionValues.Count}");
            filterBuilder.ExpressionValues.Add(filter.FilterValue.SingleOrDefault()?.ToString() ?? throw new NullReferenceException("Invalid Filter"));

            return filterBuilder;
        }

    }
}
