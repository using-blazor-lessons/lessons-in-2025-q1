using Fluxor;
using UsingBlazor.Client.Services;

namespace UsingBlazor.Client.Pages;

public class WatcherEffects(HubConnectionService hubConnectionService)
{
    [EffectMethod]
    public async Task KeyPressed(WatcherActions.KeyPressed keyPressed, IDispatcher _) => await hubConnectionService.PublishToBackendAsync(keyPressed);
}