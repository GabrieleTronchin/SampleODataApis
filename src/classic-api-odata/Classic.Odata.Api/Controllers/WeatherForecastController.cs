using Classic.Odata.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Classic.Odata.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(IQueryService<WeatherForecast> service) : ODataController
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(ODataQueryOptions<WeatherForecast> options)
        {
            return Ok(await service.Get(options));
        }

    }
}
