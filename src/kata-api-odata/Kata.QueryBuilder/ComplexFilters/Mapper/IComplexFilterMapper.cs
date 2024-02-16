using AutoMapper;

namespace Kata.QueryBuilder.ComplexFilters.Mapper
{
    public interface IComplexFilterMapper<T> where T : class
    {
        IMapper Mapper { get; }
    }
}
