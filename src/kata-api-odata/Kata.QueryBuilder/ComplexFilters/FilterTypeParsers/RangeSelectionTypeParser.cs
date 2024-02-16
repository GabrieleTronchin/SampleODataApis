using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;

namespace Kata.QueryBuilder.ComplexFilters.FilterTypeParsers
{
    public class RangeSelectionTypeParser : IFilterTypeParser
    {


        public FilterBuilder Parse(FilterBuilder filterBuilder, Filter filter)
        {
            if (filter.FilterValue.Count != 2) throw new InvalidOperationException($"Invalid Filter input parameter. {nameof(RangeSelectionTypeParser)} filter must contains 2 values.");
            filterBuilder.Expression.Append($"{filter.FilterColumn} >= {filter.FilterValue[0]} AND {filter.FilterColumn} <= {filter.FilterValue[1]}");
            return filterBuilder;
        }

    }
}
