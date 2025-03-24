
# "Using Blazor" Lessons in 2025 Q1

Published under MIT No AI Licence:

- [License](LICENSE.md)

## Lesson 02 - State Management with the Flux pattern

It is important to clearify the role of Redux, because there is often a lot of confusion about it.
First, Redux is an implementation of the Flux pattern. It has its roots in the apple/swift ecosystem, so I have been told (not verified).
Redux was very successful in the successful SPA frameworks. Secend, it came with the Redux Dev Tools, which is a independent powerful tool, not an inherent part.

### 00 - The Flux Pattern

Itself, the Flux pattern defines a technology independent concept to manage states in a standardized control loop. Later the "inner" control loop was extended by the "outer" control loop.

The inner control loop defines
- Components - produce Actions by user interactions or technical triggers.
- Dispatcher - takes occuring Actions and dispatches them to consumers.
- Reducers - the consumers of the inner control loop.
- Stores - Immutable in memory states, used as data sources for Components.

Remarks: Who feels reminded of WPF MVVM is not wrong. The intention of decoupling is the same.

The outer control loop defines
- Effects - consumers (and producers) to standardize remote requests and their reasults into the flux pattern.


![Screenshot 00](lesson_02_statemanagement_with_flux/00_Flux.drawio.png)


Its important to understand the timing behind. The inner control loop is local and always faster than the remote working outer control loop.
For the typical timeline of any use case imagine following scenario:

1. A user defines some filters and sends out a query to a bigger database in the backend. Therfore an action is dispached in the code, containing the filter's data.
2. ALL Reducers which match the action type receive the action and apply it to the related stores. When more than one store should be affected, you need one reducer per store.
3. Related stores are updated and bound components are marked for rerendering. In our case, the data is not fetched till now. So a boolean called "showSpinner" is set to true.
4. Components are rendered and will show a spinner instead of a datatable.
5. When (2) is performed ALL effects which match the action type receive the action as well.
6. Responses to these request will take some time and the results are dispatched again. Trick: Every response is an action, same like 1.
7. Same like 2.
8. Same like 3, but "showSpinner" is set to false.
9. Components are rendered and will show the fetched data.

### 01 - NuGet - Fluxor

Starting fram the heavy classic Redux there are a lot of implementations of even more lightweight frameworks like "Zustand" in React or embedded like in Angular.

And within the Blazor world? We are very happy! We got Fluxor!
You find this gem on [GitHub Fluxor](https://github.com/mrpmorris/Fluxor) and on [nuget.org](https://www.nuget.org/packages/Fluxor).

![Screenshot 01](lesson_02_statemanagement_with_flux/01_nuget_fluxor_blazor_web.png)

**Auto Mode**
Add the Fluxor to both of your project `UsingBlazor` and `UsingBlazor.Client`.

### 02 - Code - Add to CommonServices

As we already prepared the `CommonServices` we have to add Fluxor once to DI.

```
using Fluxor;

namespace UsingBlazor.Client;

public static class CommonServices
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var currentAssembly = typeof(Program).Assembly;
        services.AddFluxor(options => options.ScanAssemblies(currentAssembly));
    }
}
```

As you see Fluxor comes with extension methods for the `IServiceCollection`. The `FluxorOptions` need to know where it should look for types with Fluxor annotations.
We have two assemblies which may contain Fluxor annotations: `UsingBlazor` and `UsingBlazor.Client`. Both will call `CommonServices.ConfigureServices`.
In both cases `typeof(Program).Assembly` deliver the `currentAssembly` correctly. This needs to be extended, so keep that in mind for later lessons.

### 03 - Run - Counter without Fluxor

Let's run the app and inspect the Counter page from Microsoft's demo pages.

1. Navigate to *Counter*.
2. Click multiple time on *Click me*. Current count is increasing as expected.

![Screenshot 02](lesson_02_statemanagement_with_flux/02_run_counter_without_fluxor_clicks.png)

3. Navigate to *Home*.
4. Navigate to *Counter*.

![Screenshot 03](lesson_02_statemanagement_with_flux/03_run_counter_without_fluxor_reset.png)

As you can see the previous data is lost.

