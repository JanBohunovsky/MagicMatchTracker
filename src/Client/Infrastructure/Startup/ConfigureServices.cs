using MagicMatchTracker.Client.Infrastructure.Layout;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace MagicMatchTracker.Client.Infrastructure.Startup;

public static class ConfigureServices
{
	extension(WebAssemblyHostBuilder builder)
	{
		public void AddServices()
		{
			builder.AddBlazor();
			builder.AddMockData();

			builder.Services.AddHttpClient(string.Empty, client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			});
		}

		private void AddBlazor()
		{
			builder.RootComponents.Add<App>("#app");
			builder.RootComponents.Add<HeadOutlet>("head::after");
		}

		private void AddMockData()
		{
			builder.Services.AddSingleton<MockDataProvider>();
		}
	}
}