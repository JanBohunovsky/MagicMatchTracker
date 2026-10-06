using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace MagicMatchTracker.Client.Infrastructure.Startup;

public static class ConfigureApp
{
	extension(WebAssemblyHost app)
	{
		public async Task ConfigureAsync()
		{
			var logger = app.Services.GetRequiredService<ILoggerFactory>();
			EnumerableExtensions.FuzzySearchLogger = logger.CreateLogger("FuzzySearch");

			var mockDataProvider = app.Services.GetRequiredService<MockDataProvider>();
			await mockDataProvider.InitializeAsync();
		}
	}
}