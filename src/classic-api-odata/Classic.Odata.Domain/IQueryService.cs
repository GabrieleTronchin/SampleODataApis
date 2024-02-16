using Microsoft.AspNetCore.OData.Query;

namespace Classic.Odata.Domain
{
    public interface IQueryService<T> where T : class
    {
        Task<IQueryable<T>> Get(ODataQueryOptions<T> options);
    }
}