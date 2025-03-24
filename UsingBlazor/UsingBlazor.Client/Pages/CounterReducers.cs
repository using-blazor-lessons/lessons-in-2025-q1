using Fluxor;

namespace UsingBlazor.Client.Pages;

public static class CounterReducers
{
    [ReducerMethod]
    public static CounterState Increment(CounterState state, CounterActions.Increment _)
        => new(clickCount: state.ClickCount + 1);
}