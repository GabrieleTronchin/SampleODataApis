using AutoMapper.AspNet.OData;
using Classic.Odata.DataModel;
using Classic.Odata.Domain.Mapper;
using Microsoft.AspNetCore.OData.Query;

namespace Classic.Odata.Domain
{
    public class WeatherForecastQueryService(IServiceMapperAccessor mapper, WeatherContext context) : IQueryService<WeatherForecast>
    {
        public async Task<IQueryable<WeatherForecast>> Get(ODataQueryOptions<WeatherForecast> options)
        {
            return await context.WeatherForecasts.GetQueryAsync(mapper.Mapper, options);
        }
    }
}
