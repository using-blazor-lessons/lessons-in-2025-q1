using Microsoft.AspNetCore.Mvc;
using UsingBlazor.Client.Pages;

namespace UsingBlazor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    [HttpGet]
    public WeatherForecast[] Get()
        => Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now).AddDays(index),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]))
            .ToArray();
}