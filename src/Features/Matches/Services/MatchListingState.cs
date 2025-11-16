using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchListingState(
	Database database,
	MatchPlayerSelectionDialogState playerSelectionDialogState,
	MatchCreationHelper matchCreationHelper,
	NavigationManager navigationManager) : StateBase
{
	private List<Match>? _matches;

	public IReadOnlyList<Match>? Matches => _matches;

	public async Task LoadMatchesAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is not null)
			return;

		_matches = await database.Matches
			.OrderByDescending(m => m.TimeStarted ?? m.CreatedAt)
			.ToListAsync(cancellationToken);
	}

	public async Task AddNewMatchAsync(Match? templateMatch = null, CancellationToken cancellationToken = default)
	{
		if (_matches is null)
			return;

		IsBusy = true;

		var match = new Match
		{
			MatchNumber = await matchCreationHelper.GetNextMatchNumberAsync(DateTimeOffset.Now.ToDateOnly(), cancellationToken),
		};

		if (templateMatch is not null)
		{
			foreach (var templateMatchParticipation in templateMatch.Participations)
			{
				match.Participations.Add(new MatchParticipation
				{
					Match = match,
					Player = templateMatchParticipation.Player,
					Deck = templateMatchParticipation.Deck,
				});
			}

			database.Matches.Add(match);
			await database.SaveChangesAsync(cancellationToken);
		}

		IsBusy = false;

		if (templateMatch is null)
		{
			var success = await playerSelectionDialogState.ShowDialogAsync(match, cancellationToken);
			if (!success)
				return;
		}

		_matches.Insert(0, match);
		navigationManager.NavigateTo($"/matches/{match.Id}");
	}

}