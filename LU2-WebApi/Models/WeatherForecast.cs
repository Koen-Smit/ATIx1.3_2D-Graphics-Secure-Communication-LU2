using System.ComponentModel.DataAnnotations;

namespace LU2_WebApi.Models;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    [Range(-20, 55)]
    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    [Required]
    public string? Summary { get; set; }
}
