using Microsoft.AspNetCore.Mvc;

namespace API_GRUPODOS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
      

        [HttpGet(Name = "GetWeatherForecast")]
        public int Get()
        {
            return 10;
        }
    }
}
