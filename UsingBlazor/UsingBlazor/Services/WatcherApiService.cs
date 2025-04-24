using Microsoft.AspNetCore.SignalR;
using System.Reactive.Subjects;
using UsingBlazor.Client.Pages;
using UsingBlazor.Hubs;

namespace UsingBlazor.Services;

public class WatcherApiService(IHubContext<WatcherHub> hubContext) : IWatcherApi
{
    private readonly Subject<WatcherActions.KeyPressed> subjectKeyPressed = new();

    public IObservable<WatcherActions.KeyPressed> KeyPressedNotifications => subjectKeyPressed;
    
    public void SendKeyPressedToBackend(WatcherActions.KeyPressed keyPressed) => subjectKeyPressed.OnNext(keyPressed);

    public async void SendNoKeyPressedToFrontend(WatcherActions.NoKeyPressed noKeyPressed) => await hubContext.Clients.All.SendAsync("PublishToFrontend", noKeyPressed);
}