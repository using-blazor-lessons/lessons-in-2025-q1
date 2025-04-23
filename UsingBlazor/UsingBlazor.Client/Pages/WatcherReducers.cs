using Fluxor;

namespace UsingBlazor.Client.Pages;

public static class WatcherReducers
{
    [ReducerMethod]
    public static WatcherState KeyPressed(WatcherState state, WatcherActions.KeyPressed keyPressed) => new(state.Created, DateTime.Now);
}
