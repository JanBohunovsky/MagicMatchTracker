using MagicMatchTracker.Features.Shared.Dialogs.DeckEdit;
using MagicMatchTracker.Features.Shared.Services;
using MagicMatchTracker.Features.Shared.Services.Scryfall;

namespace MagicMatchTracker.Features.Shared;

public static class ConfigureSharedFeature
{
	public static IServiceCollection AddSharedFeature(this IServiceCollection services)
	{
		services.AddScoped<DeckEditDialogState>();

		services.AddHttpClient<ScryfallClient>(client =>
		{
			client.BaseAddress = new Uri("https://api.scryfall.com");
			client.DefaultRequestHeaders.Add("Accept", "application/json;q=0.9,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("User-Agent", $"MagicMatchTracker/{GetApplicationVersion()}");
		});

		services.AddHttpClient<ImageCachingService>();

		return services;
	}

	private static string GetApplicationVersion()
	{
		// TODO: Figure out versioning process.
		var now = DateTimeOffset.Now;
		return $"1.0-dev{now:yyyyMMdd}";
	}
}