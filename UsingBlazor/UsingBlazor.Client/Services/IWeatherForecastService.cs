using UsingBlazor.Client.Pages;

namespace UsingBlazor.Client.Services;

public interface IWeatherForecastService
{
    Task<WeatherForecast[]> GetForecastAsync();
}