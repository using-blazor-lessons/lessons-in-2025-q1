
# "Using Blazor" Lessons in 2025 Q1

Published under MIT No AI Licence:

- [License](LICENSE.md)

## Lesson 03 - Controllers in Blazor Auto Mode

A fundamental of today's web technology is the concept of REST communication with controllers. [Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-9.0)
provides a documentation for the usage in Blazor. As Auto Mode effects the DI behavior, so some changes are neccessary to get preconfigured ´HttpClient´ instances like expected.

### 00 - Code - Prepare UsingBlazor client project

For the lesson we capture the Weather Forecast page and make the essencial type available for `UsingBlazor`:
1. Move `Weather.razor` from `UsingBlazor/Components/Pages` to `UsingBlazor.Client/Pages`.
2. Next, we extract the definiton of the `WeatherForecast` into a new file `WeatherForecast.cs` in `UsingBlazor.Client/Pages`.
3. Change its signature to `public readonly record struct`.
4. Remove the getters, add a default and a parameterized constructor.

Expected result:
```
namespace UsingBlazor.Client.Pages;

public readonly record struct WeatherForecast
{
    public DateOnly Date { get; init; }

    public int TemperatureC { get; init; }

    public string? Summary { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public WeatherForecast()
    {
        Date = DateOnly.FromDateTime(DateTime.Now);
        Summary = string.Empty;
    }

    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }
}
```

### 01 - Code - Add the controller to UsingBlazor server project

In `UsingBlazor`:
1. Add the new solution folder `Controllers` to `UsingBlazor` in the solution explorer.
2. Right-click > Add > Controller... > Select "API Controller - Empty" and name it `WeatherForecastController`.
3. Next, copy the `summaries` array of the `Weather.razor/OnInitializedAsync` to the new controller.
4. In the controller create a `HttpGet` annotated `Get` method around the copied code which returns `WeatherForecast[]`.

Expected result:
```
using Microsoft.AspNetCore.Mvc;
using UsingBlazor.Client.Pages;

namespace UsingBlazor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" ];

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
```

5. Add `builder.Services.AddControllers()` and `app.MapControllers()` to the servers `Program.cs`. **DO NOT** add `builder.Services.AddHttpClient()` like described in examples/tutorials.

Expected result:
```
using UsingBlazor.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();

// Setup DI in Blazor Auto applications for services used on both sides.
UsingBlazor.Client.CommonServices.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(UsingBlazor.Client._Imports).Assembly);
app.MapControllers();

app.Run();
```

### 03 - Code - Configure DI for HttpClient

[Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-9.0) provides a lot of information about the two phases of DI, the occuring exception and counter measures.
Unluckily, the solution according that page recommends for the `HttpClient` to multiple registerations, going transient etc.

#### Better
As a better way it turned out to use the `NavigationManager`. It provides always a valid `BaseUri` - while server-side preredering and later in client-side rendering as well.
Add these lines to the service registrations in `UsingBlazor.Client/CommonServices`

```
services.AddScoped<HttpClient>(serviceProvider =>
{
    // Use the server's base address for API calls in Server render mode.
    var navigationManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
    return new() { BaseAddress = new Uri(navigationManager.BaseUri) };
});
```

This prevents a lot of technical code which otherwise increases complexity of all your `.razor`-pages.

### 04 - Code - Create and Register the REST service.

Next, we add a scoped service which wraps the REST call to the `WeatherForecastController`.

In `UsingBlazor.Client`:
1. Add a subfolder named `Services` in `UsingBlazor.Client`.
2. Add an `interface` named `IWeatherForecastService`.
3. Add the method definition `Task<WeatherForecast[]> GetForecastAsync();` to it.
4. Add a `class` named `WeatherForecastService`. Implement there `IWeatherForecastService` by using the route of the `WeatherForecastController`. It should be `"api/WeatherForecast`.

Expected `IWeatherForecastService.cs`:

```
using UsingBlazor.Client.Pages;

namespace UsingBlazor.Client.Services;

public interface IWeatherForecastService
{
    Task<WeatherForecast[]> GetForecastAsync();
}
```

Expected `WeatherForecastService.cs`:

```
using System.Net.Http.Json;
using UsingBlazor.Client.Pages;

namespace UsingBlazor.Client.Services;

public class WeatherForecastService(HttpClient http) : IWeatherForecastService
{
    public async Task<WeatherForecast[]> GetForecastAsync()
        => await http.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast") ?? [];
}
```

5. Now it is time to register the new service. Add `services.AddScoped<IWeatherForecastService, WeatherForecastService>();` to `UsingBlazor.Client/CommonServices.cs`.

Expected `CommonServices.cs`:

```
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
    }
}
```

### 05 - Code - Using the service.

Finally, we can use the new service. Therefore:

1. Inject the new service into the `weather.razor`.
2. Rewrite the `@code` section, so that the new service is used to populate `forecasts` when `OnInitializedAsync` is called.

Expected `weather.razor`:

```
@page "/weather"

@using UsingBlazor.Client.Services;
@attribute [StreamRendering]
@inject IWeatherForecastService WeatherForcastService

<PageTitle>Weather</PageTitle>

<h1>Weather</h1>

<p>This component demonstrates showing data.</p>

@if (forecasts == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th aria-label="Temperature in Celsius">Temp. (C)</th>
                <th aria-label="Temperature in Farenheit">Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in forecasts)
            {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private WeatherForecast[]? forecasts;

    protected override async Task OnInitializedAsync()
        => forecasts = await WeatherForcastService.GetForecastAsync();
}
```

**Run and test. The data will update whenever you reload the page.**