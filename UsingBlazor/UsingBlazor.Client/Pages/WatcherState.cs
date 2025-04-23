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