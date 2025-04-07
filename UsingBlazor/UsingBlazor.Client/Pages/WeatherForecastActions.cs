namespace UsingBlazor.Client.Pages;

public static class WeatherForecastActions
{
    public record struct Request();

    public record struct Response(WeatherForecast[] Forecasts);
}