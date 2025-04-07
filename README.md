
# "Using Blazor" Lessons in 2025 Q1

Published under MIT No AI Licence:

- [License](LICENSE.md)

## Lesson 04 - State Management with Effects

After learning about the Flux Pattern with Fluxor in Lesson 02 and implementing the `WeatherForecastService` in Lesson 03, now it is on you to put both together by extending your state management with **Effects**.
Effect are similar to reducers, but working on remote services instead of the Store. When Effects get their data by calling controllers, it is very transparent what is going on. Lets go.

### 00 - Code - Create the State.

First, a State is required.

In `UsingBlazor.Client`:
1. Add a new `class` named `WeatherForecastState` into `UsingBlazor.Client/Pages`.
2. Annotate `WeatherForecastState` with `[FeatureState]`.
3. Add the property `public WeatherForeCast[] Forecasts { get; }`.
4. Add the property `public bool Pending { get; }`.
5. Add a default constructor which initializes `Forecasts` as empty array and `Pending` as `false`.
6. Add a parameterized constructor which initializes `Forecasts` and `Pending` by an argument.

Expected result:
```
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
```

### 01 - Code - Create the Actions

Second, we need a meaningful Api.

In `UsingBlazor.Client/Pages`:
1. Add the class `WeatherForecastActions` and change its signature to `public static class WeatherForecastActions`.
2. Inside add `Request` and `Response`, each as `public record struct`.
3. Add the argument `WeatherForecast[] Forecasts` to `Response`.

Expected `WeatherForecastActions.cs`:
```
namespace UsingBlazor.Client.Pages;

public static class WeatherForecastActions
{
    public record struct Request();

    public record struct Response(WeatherForecast[] Forecasts);
}
```

### 02 - Code - Create the Reducers

All store changes go through the Reducers, any result of Effects too. So the whole Api should be covered.

In `UsingBlazor.Client/Pages`:
1. Add the class `WeatherForecastReducers` and change its signature to `public static class WeatherForecastActions`.
2. Add the methods `Request` and `Response`, each with the signature `public static`. Returntype is always `WeatherForecastState`, the first argument `state` as well. The second argument is the action.
3. `Request` returns a new state with `forecasts: state.Forecasts, pending: true`.
4. `Response` returns a new state with `forecasts: response.Forecasts, pending: false`. 

Expected `WeatherForecastReducers.cs`:
```
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
```

### 03 - Code - Using the State in the Page

Already now, the `weather.razor`-page can be changed to use the `WeatherForecastState`.

In `weather.razor`:
1. Add `@using Fluxor`.
2. Add `@inherits Fluxor.Blazor.Web.Components.FluxorComponent` which registers the page in Fluxor.
3. Add `@inject IState<WeatherForecastState> WeatherForecastState`. Now we can use the Store to modify (and simplify) the remaining code.
4. Do not forget to `@inject IDispatcher Dispatcher`.
5. Next, delete the local variable `forecasts`. Your IDE should highlight you the three former usages of it.
6. Replace `@if (forecasts == null)` with `@if (WeatherForecastState.Value.Pending)`. A nasty null-check less.
7. Replace ` @foreach (var forecast in forecasts)` with `@foreach (var forecast in WeatherForecastState.Value.Forecasts)`. Same like before but iterating through the State's data.
8. Delete `forecasts = await WeatherForcastService.GetForecastAsync();`. As you can see `OnInitializedAsync` is now empty.
9. Fluxor will handle all async behavior. So instead of using `OnInitializedAsync` we now can use `OnInitialized` to dispatch a `WeatherForecastActions.Request` when the page is initialized.
10. At last delete `@attribute [StreamRendering]` because rerendering is now triggered by Fluxor. Then add `@rendermode InteractiveAuto` to enable Auto Mode.

Expected `Weather.razor`:
```
@inherits Fluxor.Blazor.Web.Components.FluxorComponent
@inject IState<WeatherForecastState> WeatherForecastState
@inject IDispatcher Dispatcher
@inject IWeatherForecastService WeatherForcastService

<PageTitle>Weather</PageTitle>

<h1>Weather</h1>

<p>This component demonstrates showing data.</p>

@if (WeatherForecastState.Value.Pending)
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
            @foreach (var forecast in WeatherForecastState.Value.Forecasts)
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
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Dispatcher.Dispatch(new WeatherForecastActions.Request());
    }
}
```

**Run and test. `Loading...` will be shown but never updates.**

### 04 - Code - Add the Effects.

The missing piece is the required Effect of course. After adding it the Weather page will act like expected.

In `UsingBlazor.Client/Pages`:
1. Add the class `WeatherForecastEffects`. Add a constructor, which gets the `IWeatherForecastService` injected and stores it in a property or field.
2. Instead you can use the new default constructur pattern as well.
3. Add a method with this signature: `public async Task RequestWeatherForcast(WeatherForecastActions.Request request, IDispatcher dispatcher)`. Annotate it with the attribute `EffectMethod`.
4. Use the `IWeatherForecastService` to request and await the api call like in Lesson 03.
5. Create a `WeatherForecastActions.Response` and use the `dispatcher` to dispatch it.

Expected `WeatherForecastEffects.cs`:

```
using Fluxor;
using UsingBlazor.Client.Services;

namespace UsingBlazor.Client.Pages;

public class WeatherForecastEffects(IWeatherForecastService weatherForecastService)
{
    [EffectMethod]
    public async Task RequestWeatherForcast(WeatherForecastActions.Request request, IDispatcher dispatcher)
    {
        var forcasts = await weatherForecastService.GetForecastAsync();
        dispatcher.Dispatch(new WeatherForecastActions.Response(forcasts));
    }
}
```

**Run and test. Data will be updated by the controller again.**

Note: Of course the data changes because it is everytime requested you enter the page again.