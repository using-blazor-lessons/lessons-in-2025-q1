using Fluxor;

namespace UsingBlazor.Client.Pages;

public static class WeatherForecastReducers
{
    [ReducerMethod]
    public static WeatherForecastState Request(WeatherForecastState state, WeatherForecastActions.Request _)
        => new(forecasts: state.Forecasts, pending: true);

    [ReducerMethod]
    public static WeatherForecastState Response(WeatherForecastState _, WeatherForecastActions.Response response)
        => new(forecasts: response.Forecasts, pending: false);
}