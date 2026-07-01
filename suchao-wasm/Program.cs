using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SuchaoLeagueWasm;
using SuchaoLeagueWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<LeagueService>();

await builder.Build().RunAsync();
