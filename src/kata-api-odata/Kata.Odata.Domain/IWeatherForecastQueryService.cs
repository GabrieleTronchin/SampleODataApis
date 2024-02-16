

namespace Kata.Odata.Domain
{
    public interface IWeatherForecastQueryService
    {
        Task<IEnumerable<dynamic>> Get(Dictionary<string, string> odata);
    }
}