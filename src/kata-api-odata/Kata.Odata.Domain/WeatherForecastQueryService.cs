using Kata.Odata.DataModel.DataReader;
using Kata.Odata.DataModel.KataQuery.QueryBuilder;

namespace Kata.Odata.Domain
{
    public class WeatherForecastQueryService(IODataQueryBuilder oDataQueryBuilder, IDataReader dataReader) : IWeatherForecastQueryService
    {

        public async Task<IEnumerable<dynamic>> Get(Dictionary<string, string> odata)
        {
            var query = await oDataQueryBuilder.CreateQuery<WeatherForecast>(odata, "testTable");

            var tSqlQuery = await oDataQueryBuilder.GetSqlQuery(query);

            return await dataReader.QueryAsync(tSqlQuery.SqlCommand, tSqlQuery.Parameters);
        }

    }
}
