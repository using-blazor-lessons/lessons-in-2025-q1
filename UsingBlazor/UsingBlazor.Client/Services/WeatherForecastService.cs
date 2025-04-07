using System.Net.Http.Json;
using UsingBlazor.Client.Pages;

namespace UsingBlazor.Client.Services;

public class WeatherForecastService(HttpClient http) : IWeatherForecastService
{
    public async Task<WeatherForecast[]> GetForecastAsync()
        => await http.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast") ?? [];
}