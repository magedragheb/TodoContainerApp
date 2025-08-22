using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BackendAPI", client => client.BaseAddress = new Uri("https://localhost:7155"))
.AddHttpMessageHandler(sp =>
    sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(authorizedUrls: ["https://localhost:7155"],
    scopes: ["api://c44fd22d-6a8e-4cbd-9e7d-f936295ce070/API.Access"]));

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BackendAPI"));

builder.Services.AddFluentUIComponents();

builder.Services.AddMsalAuthentication(options =>
{
    // builder.Configuration.Bind("EntraId", options.ProviderOptions.Authentication);
    options.ProviderOptions.Authentication.Authority = "https://login.microsoftonline.com/f810dedf-bfca-4500-b934-069e265d3f04";
    options.ProviderOptions.Authentication.ClientId = "3f46fdcf-c04d-4cee-aa74-aa001697a3cc";
    options.ProviderOptions.Authentication.RedirectUri = "https://localhost:5001/authentication/login-callback";
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://c44fd22d-6a8e-4cbd-9e7d-f936295ce070/API.Access");
    options.ProviderOptions.LoginMode = "redirect";
    options.ProviderOptions.Authentication.NavigateToLoginRequestUrl = true;
});

await builder.Build().RunAsync();
