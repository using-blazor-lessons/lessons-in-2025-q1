using Fluxor;
using UsingBlazor.Client.Services;

namespace UsingBlazor.Client.Pages;

public class WeatherForecastEffects(IWeatherForecastService weatherForecastService)
{
    [EffectMethod]
    public async Task RequestWeatherForcast(WeatherForecastActions.Request _, IDispatcher dispatcher)
    {
        var forcasts = await weatherForecastService.GetForecastAsync();
        dispatcher.Dispatch(new WeatherForecastActions.Response(forcasts));
    }
}