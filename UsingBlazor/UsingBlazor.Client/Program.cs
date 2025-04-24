using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UsingBlazor.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Setup DI in Blazor Auto applications for services used on both sides.
UsingBlazor.Client.CommonServices.ConfigureServices(builder.Services);

var host = builder.Build();
var hubConnectionService = host.Services.GetRequiredService<HubConnectionService>();
await host.RunAsync();