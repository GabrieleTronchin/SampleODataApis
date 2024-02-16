using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;
using SqlKata;

namespace Kata.QueryBuilder.ComplexFilters
{
    public interface IKataComplexFilterManager : IComplexFilterManager
    {
        Task<Query> ApplyFilters<T>(Query entityQuery, string targetEntity, IEnumerable<ComplexFilter>? defaultFilters = default) where T : class;

    }
}