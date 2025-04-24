using Fluxor;

namespace UsingBlazor.Client.Pages;

public static class WatcherReducers
{
    [ReducerMethod]
    public static WatcherState KeyPressed(WatcherState state, WatcherActions.KeyPressed _) => new(state.Created, DateTime.Now);

    [ReducerMethod]
    public static WatcherState NoKeyPressed(WatcherState state, WatcherActions.NoKeyPressed _) => new(state.Created, DateTime.Now);
}