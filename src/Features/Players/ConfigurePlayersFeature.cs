using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Features.Players.Pages.Detail;
using MagicMatchTracker.Features.Players.Pages.Listing;

namespace MagicMatchTracker.Features.Players;

public static class ConfigurePlayersFeature
{
	public static IServiceCollection AddPlayersFeature(this IServiceCollection services)
	{
		services.AddScoped<PlayerListingState>();
		services.AddScoped<PlayerDetailState>();
		services.AddScoped<PlayerEditDialogState>();

		return services;
	}
}