using MagicMatchTracker.Features.Shared.Dialogs.DeckEdit;
using MagicMatchTracker.Features.Shared.Options;
using MagicMatchTracker.Features.Shared.Services;
using MagicMatchTracker.Features.Shared.Services.Scryfall;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Shared;

public static class ConfigureSharedFeature
{
	public static IServiceCollection AddSharedFeature(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<DeckEditDialogState>();

		services.AddHttpClient<ScryfallClient>((serviceProvider, client) =>
		{
			var versionProvider = serviceProvider.GetRequiredService<ApplicationVersionProvider>();

			client.BaseAddress = new Uri("https://api.scryfall.com");
			client.DefaultRequestHeaders.Add("Accept", "application/json;q=0.9,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("User-Agent", $"MagicMatchTracker/{versionProvider.Version}");
		});

		services.AddHttpClient<ImageCachingService>();

		services.Configure<ChangelogOptions>(configuration.GetSection(ChangelogOptions.SectionName));

		return services;
	}
}