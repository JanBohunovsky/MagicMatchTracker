using MagicMatchTracker.Client.Infrastructure.Startup;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.AddServices();

var app = builder.Build();
await app.ConfigureAsync();

await app.RunAsync();