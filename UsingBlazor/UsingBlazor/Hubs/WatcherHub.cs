using Microsoft.AspNetCore.SignalR;
using UsingBlazor.Client.Pages;
using UsingBlazor.Services;

namespace UsingBlazor.Hubs;

public class WatcherHub(IWatcherApi watcher) : Hub
{
    public void PublishToBackend(WatcherActions.KeyPressed keyPressed) => watcher.SendKeyPressedToBackend(keyPressed);
}