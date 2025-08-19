using MagicMatchTracker.Features.Matches.Services;

namespace MagicMatchTracker.Features.Matches;

public static class ConfigureMatchesFeature
{
	public static IServiceCollection AddMatchesFeature(this IServiceCollection services)
	{
		services.AddScoped<MatchListingState>();
		services.AddScoped<MatchDetailState>();
		services.AddScoped<MatchEditState>();
		services.AddTransient<MatchPlayerSearchProvider>();
		services.AddTransient<MatchDeckSearchProvider>();

		return services;
	}
}