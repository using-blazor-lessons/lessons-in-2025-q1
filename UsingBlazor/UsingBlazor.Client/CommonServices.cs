using Fluxor;
using UsingBlazor.Client.Pages;

namespace UsingBlazor.Client;

public static class CommonServices
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddFluxor(options => options.ScanAssemblies(typeof(Counter).Assembly));
    }
}