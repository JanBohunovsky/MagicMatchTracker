using MagicMatchTracker.Features.Matches.Dialogs.DeckSelect;
using MagicMatchTracker.Features.Matches.Dialogs.DetailsEdit;
using MagicMatchTracker.Features.Matches.Dialogs.EndTransition;
using MagicMatchTracker.Features.Matches.Dialogs.ParticipationDetailsEdit;
using MagicMatchTracker.Features.Matches.Dialogs.ParticipationEndStateEdit;
using MagicMatchTracker.Features.Matches.Dialogs.PlayersEdit;
using MagicMatchTracker.Features.Matches.Dialogs.StartTransition;
using MagicMatchTracker.Features.Matches.Pages.Detail;
using MagicMatchTracker.Features.Matches.Pages.Listing;

namespace MagicMatchTracker.Features.Matches;

public static class ConfigureMatchesFeature
{
	public static IServiceCollection AddMatchesFeature(this IServiceCollection services, IConfiguration configuration)
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
			.AddScoped<MatchDeckSearchProvider>();

		services.Configure<MatchListingOptions>(configuration.GetSection(MatchListingOptions.SectionName));

		return services;
	}
}