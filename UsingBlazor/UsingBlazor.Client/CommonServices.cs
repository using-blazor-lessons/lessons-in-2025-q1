using Fluxor;
using UsingBlazor.Client.Pages;
using UsingBlazor.Client.Services;

namespace UsingBlazor.Client;

public static class CommonServices
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<HttpClient>(serviceProvider =>
        {
            // Use the server's base address for API calls in Server render mode.
            var navigationManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
            return new() { BaseAddress = new Uri(navigationManager.BaseUri) };
        });
        services.AddFluxor(options => options.ScanAssemblies(typeof(Counter).Assembly));
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped<HubConnectionService>();
    }
}