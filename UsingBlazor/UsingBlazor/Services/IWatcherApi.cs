using UsingBlazor.Client.Pages;

namespace UsingBlazor.Services;

public interface IWatcherApi
{
    public IObservable<WatcherActions.KeyPressed> KeyPressedNotifications { get; }

    public void SendKeyPressedToBackend(WatcherActions.KeyPressed keyPressed);

    public void SendNoKeyPressedToFrontend(WatcherActions.NoKeyPressed noKeyPressed);
}