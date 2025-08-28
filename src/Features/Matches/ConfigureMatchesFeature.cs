using MagicMatchTracker.Features.Matches.Services;

namespace MagicMatchTracker.Features.Matches;

public static class ConfigureMatchesFeature
{
	public static IServiceCollection AddMatchesFeature(this IServiceCollection services)
	{
		services
			.AddScoped<MatchListingState>()
			.AddScoped<MatchDetailState>()
			.AddScoped<MatchEditState>()
			.AddScoped<MatchPlayerSelectionState>();

		return services;
	}
}