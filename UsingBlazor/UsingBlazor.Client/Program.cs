using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Setup DI in Blazor Auto applications for services used on both sides.
UsingBlazor.Client.CommonServices.ConfigureServices(builder.Services);

await builder.Build().RunAsync();
