using Microsoft.FluentUI.AspNetCore.Components;
using TodoBlazorServer.Components;

var builder = WebApplication.CreateBuilder(args);
var backendUrl = builder.Configuration["backendurl"] ?? throw new Exception("missing BackendUrl");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();
builder.Services.AddHttpClient("BackendAPI", client =>
client.BaseAddress = new Uri(backendUrl));
// .AddHttpMessageHandler<AuthorizationMessageHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
