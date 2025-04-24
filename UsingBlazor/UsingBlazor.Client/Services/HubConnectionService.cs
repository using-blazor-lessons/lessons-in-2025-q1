using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using UsingBlazor.Client.Pages;

namespace UsingBlazor.Client.Services;

public class HubConnectionService
{
    private readonly IDispatcher dispatcher;

    public HubConnection HubConnection { get; init; }

    public HubConnectionService(NavigationManager navigationManager, IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;

        HubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/PublishToBackend"))
            .WithAutomaticReconnect()
            .Build();

        HubConnection.On<WatcherActions.NoKeyPressed>("PublishToFrontend", PublishToFrontend);
    }

    private void PublishToFrontend(WatcherActions.NoKeyPressed noKeyPressed)
    {
        Console.WriteLine($"Received {noKeyPressed}");
        dispatcher.Dispatch(noKeyPressed);
    }

    public async Task PublishToBackendAsync(WatcherActions.KeyPressed keyPressed)
    {
        if (HubConnection.State is not HubConnectionState.Connected)
            await HubConnection.StartAsync();

        await HubConnection.SendAsync("PublishToBackend", keyPressed);
    }
}