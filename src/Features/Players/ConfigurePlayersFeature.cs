using MagicMatchTracker.Features.Players.Services;

namespace MagicMatchTracker.Features.Players;

public static class ConfigurePlayersFeature
{
	public static void AddPlayersFeature(this IServiceCollection services)
	{
		services.AddScoped<PlayerListingState>();
		services.AddScoped<PlayerEditState>();
	}
}