using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;
using Kata.QueryBuilder.ComplexFilters.FilterComparators;
using Microsoft.Extensions.DependencyInjection;

namespace Kata.QueryBuilder.ComplexFilters.FilterTypeParsers
{
    public class DefaultFilterTypeParser : IFilterTypeParser
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultFilterTypeParser(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public FilterBuilder Parse(FilterBuilder filterBuilder, Filter filter)
        {
            if (!filter.FilterValue.Any()) throw new InvalidOperationException($"Invalid Filter input parameter. {nameof(DefaultFilterTypeParser)} filter admit only 1 value.");

            var comparatorType = typeof(DefaultComparator).Assembly.DefinedTypes.FirstOrDefault(x => x.Name == $"{Enum.GetName(typeof(LogialComparator), filter.FilterComparator)}Comparator") ?? typeof(DefaultComparator);
            IComparator comparator = (IComparator)_serviceProvider.GetRequiredService(comparatorType);
            filterBuilder = comparator.ComposeExpression(filterBuilder, filter);

            return filterBuilder;
        }

    }
}
