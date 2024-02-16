using Kata.Odata.Api.Helper;
using Kata.Odata.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Kata.Odata.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(HttpRequest request, IWeatherForecastQueryService queryService) : ControllerBase
    {

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            return Ok(await queryService.Get(request.GetODataOption()));
        }
    }
}
