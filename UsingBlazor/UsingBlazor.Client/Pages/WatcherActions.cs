using Microsoft.AspNetCore.Components.Web;

namespace UsingBlazor.Client.Pages;

public static class WatcherActions
{
    public record struct KeyPressed(KeyboardEventArgs KeyboardEventArgs);

    public record struct NoKeyPressed(DateTime TimeStamp);
}