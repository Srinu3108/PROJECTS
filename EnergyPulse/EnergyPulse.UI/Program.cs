using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using EnergyPulse.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazorise();
builder.Services.AddBootstrapProviders();
builder.Services.AddFontAwesomeIcons();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5015/")
});

await builder.Build().RunAsync();