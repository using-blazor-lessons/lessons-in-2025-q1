
# "Using Blazor" Lessons in 2025 Q1

Published under MIT No AI Licence:

- [License](LICENSE.md)

## Lesson 05 - Timing, Reducers-only

It is important to get a feeling about the time behavior when working with stores. Therefor we build a new page component - the Watcher. It will use the `DateTime` and `TimeSpan` an 
keyboard inputs to measure the time costs of simple store updates without effects first.

### 00 - Setup - Add the Watcher, its Actions, State and Reducers.

In `UsingBlazor.Client`:
1. Add a new `Razor Component` named `Watcher.razor` into `UsingBlazor.Client/Pages`.
2. Add a new `class` named `WatcherActions.cs` into `UsingBlazor.Client/Pages`.
3. Add a new `class` named `WatcherState.cs` into `UsingBlazor.Client/Pages`.
4. Add a new `class` named `WatcherReducers.cs` into `UsingBlazor.Client/Pages`.

### 01 - Code - Routing

The `Watcher.razor` starts mostly empty and without a route to it.
```
<h3>Watcher</h3>

@code {

}
```

At top add the pages route:
`@page "/watcher"`

Additionally we add the rendermode as well:
`@rendermode InteractiveAuto`

Now we add the new route to the `NavMenu.razor` in `UsingBlazor/Components/Layout` below the existing entries:
```
<div class="nav-item px-3">
    <NavLink class="nav-link" href="watcher">
        <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Watcher
    </NavLink>
</div>
```

You can already run the app and enjoy your new component.

### 02 - Code - The State.

To observe a states timing behavior we need a state.

1. As before we have to annotate the `WatcherState` with `[FeatureState]`.
2. Then add two pulic properties: `Created` and `Updated`, both of type `DateTime`.
3. Add a default and a parameterized constructor.

The result should look like this:

```
using Fluxor;

namespace UsingBlazor.Client.Pages;

[FeatureState]
public class WatcherState
{
    public DateTime Created { get; init; }
    public DateTime Updated { get; init; }

    public WatcherState()
    {
        Created = DateTime.Now;
        Updated = DateTime.Now;
    }

    public WatcherState(DateTime created, DateTime updated)
    {
        Created = created;
        Updated = updated;
    }
}
```

### 03 - Code - The Actions.

We want to compute when a key is pressed. Like before:

1. Make the `WatcherActions` `public static`.
2. Add `public record struct KeyPressed(KeyboardEventArgs KeyboardEventArgs);` as the required action.

`KeyboardEventArgs` belongs to namespace `Microsoft.AspNetCore.Components.Web` and we will use it to capture and compute the pressed keys.

### 04 - Code - The Reducers.

Bunding the action and the state together result in the necessary `[ReducerMethod]`. Like before:

1. Make the `WatcherReducers` `public static`.
2. Add this simple reducer method to it:
```
[ReducerMethod]
public static WatcherState KeyPressed(WatcherState state, WatcherActions.KeyPressed keyPressed) => new(state.Created, DateTime.Now);
```

Done.

### 05 - Code - The View.

Let's go back to `Watcher.razor`. To design the view we use a simple HTML-Table to show the data we want to see.
Add this pre-defined table below `<h3>Watcher</h3>`:

```
<table tabindex="0" cellpadding="10px" @ref="tableReference" @onkeypress=@OnKeyPressed>
    <tr>
        <th>Context</th>
        <th>DateTime</th>
        <th>Ticks since Store</th>
        <th>Ticks since Key pressed</th>
    </tr>
    <tr>
        <td>Store initialized</td>
        <td align="right"></td>
        <td align="right"></td>
        <td align="right" />
    </tr>
    <tr>
        <td>Page initialized</td>
        <td align="right"></td>
        <td align="right"></td>
        <td align="right" />
    </tr>
    <tr>
        <td>Key pressed</td>
        <td align="right"></td>
        <td align="right"></td>
        <td align="right"></td>
    </tr>
    <tr>
        <td>Store updated</td>
        <td align="right"></td>
        <td align="right"></td>
        <td align="right"></td>
    </tr>
    <tr>
        <td>Store.StateChanged</td>
        <td align="right"></td>
        <td align="right"></td>
        <td align="right"></td>
    </tr>
</table>
```

As long `tableReference` and `OnKeyPressed` cannot be resolved, the you can not run the app. 
So add `private ElementReference tableReference;` and `private void OnKeyPressed(KeyboardEventArgs args) { }` to the `@code { }` section.

Now you can run the app again.

### 06 - Code - Using the Store and local data in the View.

1. We start by inheriting the component from `@inherits Fluxor.Blazor.Web.Components.FluxorComponent`.
2. Then inject the new store with `@inject IState<WatcherState> WatcherState`.
3. Now we can bind the first two values into the table. To get a high accuracy in the displayed strings, we will use `.ToString("yyyy-MM-dd HH:mm:ss.fff")` to get the milliseconds.
4. To measure time, we need add three additional local variables which are simply initialized to UNIX-Epoch. Add them to the `@code` section and resolve the in the table.

```
private DateTime initializedPageAt = DateTime.UnixEpoch;
private DateTime keyPressed = DateTime.UnixEpoch;
private DateTime storeStateChanged = DateTime.UnixEpoch;
```

The table should look like this:

```
<table tabindex="0" cellpadding="10px" @ref="tableReference" @onkeypress=@OnKeyPressed>
    <tr>
        <th>Context</th>
        <th>DateTime</th>
        <th>Ticks since Store</th>
        <th>Ticks since Key pressed</th>
    </tr>
    <tr>
        <td>Store initialized</td>
        <td align="right">@WatcherState.Value.Created.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right"></td>
        <td align="right" />
    </tr>
    <tr>
        <td>Page initialized</td>
        <td align="right">@initializedPageAt.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right"></td>
        <td align="right" />
    </tr>
    <tr>
        <td>Key pressed</td>
        <td align="right">>@keyPressed.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right"></td>
        <td align="right"></td>
    </tr>
    <tr>
        <td>Store updated</td>
        <td align="right">@WatcherState.Value.Updated.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right"></td>
        <td align="right"></td>
    </tr>
    <tr>
        <td>Store.StateChanged</td>
        <td align="right">@storeStateChanged.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right"></td>
        <td align="right"></td>
    </tr>
</table>
```

### 06 - Code - Time calculations.

We need some calculations to get meaningful data for us humans. Therefore we add two simple helper methods to the `@code` section:

```
private long TicksSinceStoreInitialized(DateTime offset) => (offset - WatcherState.Value.Created).Ticks;
private long TicksSinceKeyPressed(DateTime offset) => (offset - keyPressed).Ticks;
```

The methods names should be documentation enough. ;-)

### 06 - Code - Dispatch KeyPressed Action.

We still miss the dispatched action. To enable this, please:

1. Inject the dispatcher with `@inject IDispatcher Dispatcher`.
2. Add `Dispatcher.Dispatch(new WatcherActions.KeyPressed(args));` to the local `OnKeyPressed` method.

If you debug now the app, you will recognize that `OnKeyPressed` is not triggered. The reason is, that the table need focus first. And yes, after each rendering.
Blazor offers us appropriate method to override and this is where we needed the `tableReference` for:

3. Override `OnAfterRenderAsync` like this:

```
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
        await tableReference.FocusAsync();
}
```

Run the app and you may use WASD to observe that the `Store updated` row is updated properly.

### 07 - Code - Collect additional data.

Still, the three local variables `initializedPageAt`, `keyPressed` and `storeStateChanged` for measurements are not set.

1. Most easy is `keyPressed`. Add `keyPressed = DateTime.Now;` into the `OnKeyPressed` before the dispatcher is called.
2. When you guess that `initializedPageAt = DateTime.Now;` should placed in the override of `OnInitialized` you are right.

Important to know and understand is, that each Fluxor Store has a useful `StateChanged`. This will be used for the last variable `storeStateChanged`.

3. Therefore add a `EventHandler` to the `WatcherState.StateChanged` event in the overridden `OnInitialized`. It assigns `storeStateChanged = DateTime.Now;`.
4. When testing you app later you will recognize, that this value is not updated. The reason is that you have to trigger a re-rendering manually from the code behind with `StateHasChanged();`.

The `OnInitialized` should look like that:

```
protected override void OnInitialized()
{
    base.OnInitialized();

    initializedPageAt = DateTime.Now;

    WatcherState.StateChanged += (sender, e) =>
    {
        storeStateChanged = DateTime.Now;
        StateHasChanged();
    };
}
```

### 08 - Code - Put all together.

Finally you can add all remaining values and calculations into the table by using `TicksSinceStoreInitialized` and `TicksSinceKeyPressed`:

```
<table tabindex="0" cellpadding="10px" @ref="tableReference" @onkeypress=@OnKeyPressed>
    <tr>
        <th>Context</th>
        <th>DateTime</th>
        <th>Ticks since Store</th>
        <th>Ticks since Key pressed</th>
    </tr>
    <tr>
        <td>Store initialized</td>
        <td align="right">@WatcherState.Value.Created.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right">@TicksSinceStoreInitialized(WatcherState.Value.Created)</td>
        <td align="right" />
    </tr>
    <tr>
        <td>Page initialized</td>
        <td align="right">@initializedPageAt.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right">@TicksSinceStoreInitialized(initializedPageAt)</td>
        <td align="right" />
    </tr>
    <tr>
        <td>Key pressed</td>
        <td align="right">@keyPressed.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right">@TicksSinceStoreInitialized(keyPressed)</td>
        <td align="right">@TicksSinceKeyPressed(keyPressed)</td>
    </tr>
    <tr>
        <td>Store updated</td>
        <td align="right">@WatcherState.Value.Updated.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right">@TicksSinceStoreInitialized(WatcherState.Value.Updated)</td>
        <td align="right">@TicksSinceKeyPressed(WatcherState.Value.Updated)</td>
    </tr>
    <tr>
        <td>Store.StateChanged</td>
        <td align="right">@storeStateChanged.ToString("yyyy-MM-dd HH:mm:ss.fff")</td>
        <td align="right">@TicksSinceStoreInitialized(storeStateChanged)</td>
        <td align="right">@TicksSinceKeyPressed(storeStateChanged)</td>
    </tr>
</table>
```

When you now run the app, the cunched number will give you a good impressing of the initialization sequence of store and page.
Also, how fast and in which sequence store changes are computed.