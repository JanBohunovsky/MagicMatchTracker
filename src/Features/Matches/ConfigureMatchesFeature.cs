using MagicMatchTracker.Features.Matches.Services;

namespace MagicMatchTracker.Features.Matches;

public static class ConfigureMatchesFeature
{
	public static IServiceCollection AddMatchesFeature(this IServiceCollection services)
	{
		services
			.AddScoped<MatchListingState>()
			.AddScoped<MatchDetailState>()
			.AddScoped<MatchDetailsEditDialogState>()
			.AddScoped<MatchPlayersEditDialogState>()
			.AddScoped<MatchDeckSelectionDialogState>()
			.AddScoped<MatchParticipationDetailsEditDialogState>()
			.AddScoped<MatchParticipationEndStateEditDialogState>()
			.AddScoped<MatchStartTransitionDialogState>()
			.AddScoped<MatchEndTransitionDialogState>()
			.AddScoped<MatchDeckSearchProvider>()
			.AddScoped<MatchNumberingHelper>();

		return services;
	}
}