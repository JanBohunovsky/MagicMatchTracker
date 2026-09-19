using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Features.Players.Pages.Detail.DeckListing;
using MagicMatchTracker.Features.Players.Pages.Detail.MatchHistory;
using MagicMatchTracker.Features.Players.Pages.Listing;

namespace MagicMatchTracker.Features.Players;

public static class ConfigurePlayersFeature
{
	public static IServiceCollection AddPlayersFeature(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<PlayerListingState>()
			.AddScoped<PlayerDeckListingState>()
			.AddScoped<PlayerMatchHistoryState>()
			.AddScoped<PlayerEditDialogState>();

		services.Configure<PlayerMatchHistoryOptions>(configuration.GetSection(PlayerMatchHistoryOptions.SectionName));

		return services;
	}
}