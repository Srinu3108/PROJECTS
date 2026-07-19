using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using EnergyPulse.UI;
using EnergyPulse.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazorise();
builder.Services.AddBootstrapProviders();
builder.Services.AddFontAwesomeIcons();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Add AuthService
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5015/")
});

await builder.Build().RunAsync();