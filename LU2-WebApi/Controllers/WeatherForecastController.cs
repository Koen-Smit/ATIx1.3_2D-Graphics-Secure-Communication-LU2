using LU2_WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LU2_WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private static List<WeatherForecast> _weatherForecasts = new List<WeatherForecast>();
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    // GET: /WeatherForecast
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return _weatherForecasts;
    }

    // POST: /WeatherForecast
    [HttpPost]
    public IActionResult Post([FromBody] WeatherForecast forecast)
    {
        if (forecast == null)
        {
            return BadRequest("Invalid weather forecast data.");
        }

        _weatherForecasts.Add(forecast);
        return CreatedAtAction(nameof(Get), new { date = forecast.Date }, forecast);
    }

    // GET: /WeatherForecast/{date}
    [HttpGet("{date}")]
    public IActionResult GetByDate(DateOnly date)
    {
        var forecast = _weatherForecasts.FirstOrDefault(f => f.Date == date);
        if (forecast == null)
        {
            return NotFound("Weather forecast for the given date not found.");
        }
        return Ok(forecast);
    }
}
