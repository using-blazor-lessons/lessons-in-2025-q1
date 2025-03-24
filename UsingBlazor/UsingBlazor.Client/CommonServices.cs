using Fluxor;

namespace UsingBlazor.Client;

public static class CommonServices
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var currentAssembly = typeof(Program).Assembly;
        services.AddFluxor(options => options.ScanAssemblies(currentAssembly));
    }
}