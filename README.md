
# "Using Blazor" Lessons in 2025 Q1

Published under MIT No AI Licence:

- [License](LICENSE.md)

## Lesson 06 - Effects with SignalR

It is time to learn how effects can be used together with SignalR. Based on the Watcher page, we will implement following features:
1. When the user does not push a button, the backend should send no-button-pressed notification to the frontend each 100 milliseconds.
2. When the user does push a button, the backend should send no notifications for 1 second.

[Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-9.0&tabs=visual-studio) provides serveral Tutorials for SignalR (Core) in Blazor.
The plus of this lesson will be, that we cover the whole roundtrip beween frontend and backend. Let's go.

### 00 - Code - Extend Actions and Reducers

The first steps are easy. In `UsingBlazor.Client/Pages`:
1. Add a new  action `NoKeyPressed` to `WatcherActions`. You can add a property `DateTime TimeStamp` to it.
2. Add a new reducer method `NoKeyPressed`to `WatcherReducers`, handling `WatcherActions.NoKeyPressend` and does exactly the same like the `KeyPressed` reducer method.

The result should look like this:
```
public static class WatcherActions
{
    public record struct KeyPressed(KeyboardEventArgs KeyboardEventArgs);

    public record struct NoKeyPressed(DateTime TimeStamp);
}

public class WatcherEffects(HubConnectionService hubConnectionService)
{
    [EffectMethod]
    public async Task KeyPressed(WatcherActions.KeyPressed keyPressed, IDispatcher _)
        => await hubConnectionService.PublishToBackendAsync(keyPressed);
}
```

Your store is now ready to manage the new action.

### 01 - Code - The Api Interface

Next we need some service in the backend, which can manage the `WatcherActions` as well. To attach later dynamically business logic we will use ReactiveX.

1. Add the NuGet package `System.Reactive (6.0.1)` and `System.Reactive.Linq (6.0.1)`.
2. Add a subfolder named `Services` to `UsingBlazor` in the Solution Explorer.
3. Add a new interface named `IWatcherApi`.
4. Add get-able property `KeyPressedNotification` of type `IObservable<WatcherActions.KeyPressed>` to it.
5. Add method `SendKeyPressedToBackend`, which handles `WatcherActions.KeyPressed`.
6. Add method `SendNoKeyPressedToFrontend`, which handles `WatcherActions.NoKeyPressed`.

Resulting code:
```
public interface IWatcherApi
{
    public IObservable<WatcherActions.KeyPressed> KeyPressedNotification { get; }

    public void SendKeyPressedToBackend(WatcherActions.KeyPressed keyPressed);

    public void SendNoKeyPressedToFrontend(WatcherActions.NoKeyPressed noKeyPressed);
}
```

### 02 - Code - Setup SignalR and WatcherHub

To enable .NET Core SignalR several parts between backend and frontend are still missing.
First you have to enable SignalR, which you can do in `UsingBlazor/Program.cs` easily.
Add this code section directly below the `builder.Services.AddControllers();`:

```
// SignalR: DI.
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});
```

Next you need to implement the `Hub` which will be managed by SignalR.

In `UsingBlazor`:
1. Add folder `Hubs` in the solution explorer.
2. Add the class `WatcherHub` to it and derive it from `Hub`.
3. Use the primary constructor to get `IWatcherApi watcher` injected.
4. Add the method `public void PublishToBackend(WatcherActions.KeyPressed keyPressed) => watcher.SendKeyPressedToBackend(keyPressed);`

PublishToBackend will be mapped to the hub's route, then forwarding `WatcherActions.KeyPressed` to the `IWatcherApi`.

Behind the scenes any `Hub` transient and just alive for a blink of an eye.
So typically an incoming notification is processed and forwarded to an instance with a longer lifetime scope.

Now it is time to map the `WatcherHub` to a route. That is arcieved in `UsingBlazor/Program.cs` by place the following snippet before
```
// SignalR: Map the Hub.
app.MapHub<WatcherHub>("/PublishToBackend");
```

Done.

### 03 - Code - Implement Watcher Api as Service

Of course the implementation of IWatcherApi is missing. Add a new class named `WatcherApiServices` into `UsingBlazor/Services` and implement `IWatcherApi`.

Implement as follows:
```
public class WatcherApiService(IHubContext<WatcherHub> hubContext) : IWatcherApi
{
    private readonly Subject<WatcherActions.KeyPressed> subjectKeyPressed = new();

    public IObservable<WatcherActions.KeyPressed> KeyPressedNotifications => subjectKeyPressed;
    
    public void SendKeyPressedToBackend(WatcherActions.KeyPressed keyPressed) => subjectKeyPressed.OnNext(keyPressed);

    public async void SendNoKeyPressedToFrontend(WatcherActions.NoKeyPressed noKeyPressed) => await hubContext.Clients.All.SendAsync("PublishToFrontend", noKeyPressed);
}
```

The new service allows you to add rx handlers later, to implement any custom business logic.

At last you have to add `WatcherApiServices` to the DI and use it. Back in `UsingBlazor/Program.cs` add

1. Add `builder.Services.AddSingleton<IWatcherApi, WatcherApiService>();` below `CommonServices` declaration.
2. Require the singleton and put it into a local variable with directly below `app.MapHub<WatcherHub>("/PublishToBackend");`.
3. At last add the following lines. They implement the business logic for the feature, defines at the lesson start.

```
var watcher = app.Services.GetRequiredService<IWatcherApi>();
var dtLastKeyPressed = DateTime.Now;
var subscription = watcher.KeyPressedNotifications.Subscribe(keyPressed => dtLastKeyPressed = DateTime.Now);
var task = Task.Run(() =>
{
    while (true)
    {
        Thread.Sleep(100);
        if ((DateTime.Now - dtLastKeyPressed).Seconds > 1)
            watcher.SendNoKeyPressedToFrontend(new WatcherActions.NoKeyPressed());
    }
});
```
Run and test the app. Still no data is exchanged.

### 04 - Code - Frontend's SignalR part

Now you have to do just two more things. The hard one first - you need a steady connection to the backend. Otherwise you will not get any `WatcherAction.NoKeyPressed` notification.

In `UsingBlazor.Client`:
1. Add the NuGet package `Microsoft.AspNetCore.SignalR.Client (9.0.4)`.
2. Add a new class named `HubConnectionService`.
3. Create a constructor, which gets `NavigationManager` the `IDispatcher` injected. The `IDispatcher` should initialize a member.
4. Now you can setup a `HubConnection` in the constructor. The `HubConnectionBuilder` defines the route to the backend's `Hub`.
5. You can attach now API elements to the resulting `HubConnection`, simply by using `On<>`. Be aware that each type is bound to one `methodName`.

```
HubConnection = new HubConnectionBuilder()
    .WithUrl(navigationManager.ToAbsoluteUri("/PublishToBackend"))
    .WithAutomaticReconnect()
    .Build();

HubConnection.On<WatcherActions.NoKeyPressed>("PublishToFrontend", PublishToFrontend);
```

6. Then add the implementation of `PublishToFrontend`. Some console logging improves your developer experience, when searching for bugs.

```
private void PublishToFrontend(WatcherActions.NoKeyPressed noKeyPressed)
{
    Console.WriteLine($"Received {noKeyPressed}");
    dispatcher.Dispatch(noKeyPressed);
}
```

7. Add a comfort method here, which automatically starts the `HubConnection` when it is not connected:

```
public async Task PublishToBackendAsync(WatcherActions.KeyPressed keyPressed)
{
    if (HubConnection.State is not HubConnectionState.Connected)
        await HubConnection.StartAsync();

    await HubConnection.SendAsync("PublishToBackend", keyPressed);
}
```

8. Dependency Injection in Auto Mode >> Please add the `HubConnectionService` as scoped in `CommonServices` with 

```
services.AddScoped<HubConnectionService>();
```

9. At last you want a continuously opened `HubConnectionService`. Therefore you above `await host.RunAsync();` in `UsingBlazor.Client/Program.cs` add

```
var hubConnectionService = host.Services.GetRequiredService<HubConnectionService>();
```

Compile and run.

### 05 - Code - The Effect

All is set, except the required effect. In `UsingBlazor.Client/Pages`, please add a new class named `WatcherEffects` and implement it as follows:

```
public class WatcherEffects(HubConnectionService hubConnectionService)
{
    [EffectMethod]
    public async Task KeyPressed(WatcherActions.KeyPressed keyPressed, IDispatcher _) => await hubConnectionService.PublishToBackendAsync(keyPressed);
}
```

Compile, run and test the WATCHER. The backend is WATCHING you. ;o)