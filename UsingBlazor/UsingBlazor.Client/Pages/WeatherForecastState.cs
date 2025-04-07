using Fluxor;

namespace UsingBlazor.Client.Pages;

[FeatureState]
public class WeatherForecastState
{
    public WeatherForecast[] Forecasts { get; }

    public bool Pending { get; }

    public WeatherForecastState()
    {
        Forecasts = [];
        Pending = false;
    }

    public WeatherForecastState(WeatherForecast[] forecasts, bool pending)
    {
        Forecasts = forecasts;
        Pending = pending;
    }
}