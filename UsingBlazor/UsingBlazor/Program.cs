using Microsoft.AspNetCore.ResponseCompression;
using UsingBlazor.Client.Pages;
using UsingBlazor.Components;
using UsingBlazor.Hubs;
using UsingBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();

// SignalR: DI.
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

// Setup DI in Blazor Auto applications for services used on both sides.
UsingBlazor.Client.CommonServices.ConfigureServices(builder.Services);

// Setup DI for backend-only.
builder.Services.AddSingleton<IWatcherApi, WatcherApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseResponseCompression();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(UsingBlazor.Client._Imports).Assembly);
app.MapControllers();

// SignalR: Map the Hub.
app.MapHub<WatcherHub>("/PublishToBackend");

var watcher = app.Services.GetRequiredService<IWatcherApi>();
var dtLastKeyPressed = DateTime.Now;
var subscription = watcher.KeyPressedNotifications.Subscribe(keyPressed => dtLastKeyPressed = DateTime.Now);
var task = Task.Run(() =>
{
    while (true)
    {
        Thread.Sleep(100);
        if ((DateTime.Now - dtLastKeyPressed).Seconds > 1)
            watcher.SendNoKeyPressedToFrontend(new WatcherActions.NoKeyPressed());
    }
});

app.Run();