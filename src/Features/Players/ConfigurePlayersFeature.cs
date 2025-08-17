using MagicMatchTracker.Features.Players.Services;

namespace MagicMatchTracker.Features.Players;

public static class ConfigurePlayersFeature
{
	public static IServiceCollection AddPlayersFeature(this IServiceCollection services)
	{
		services.AddScoped<PlayerListingState>();
		services.AddScoped<PlayerDetailState>();
		services.AddScoped<PlayerEditState>();
		services.AddScoped<DeckEditState>();

		return services;
	}
}