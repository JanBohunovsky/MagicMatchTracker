using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchListingState(
	Database database,
	MatchPlayerSelectionDialogState playerSelectionDialogState,
	MatchNumberingHelper matchNumberingHelper,
	NavigationManager navigationManager) : StateBase
{
	private List<Match>? _matches;

	public IReadOnlyList<Match>? Matches => _matches;

	public async Task LoadMatchesAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is not null)
			return;

		var today = DateOnly.Today;
		_matches = await database.Matches
			.Include(m => m.Participations)
			.OrderByDescending(m => m.TimeStarted != null ? DateOnly.FromDateTime(m.TimeStarted.Value.Date) : today)
			.ThenByDescending(m => m.MatchNumber)
			.ToListAsync(cancellationToken);
	}

	public async Task AddNewMatchAsync(Match? templateMatch = null, CancellationToken cancellationToken = default)
	{
		if (_matches is null)
			return;

		IsBusy = true;

		var match = new Match
		{
			MatchNumber = await matchNumberingHelper.GetNextMatchNumberAsync(DateOnly.Today, cancellationToken),
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
		}

		IsBusy = false;

		var success = await playerSelectionDialogState.ShowDialogAsync(match, cancellationToken);
		if (!success)
			return;

		_matches.Insert(0, match);
		navigationManager.NavigateTo($"/matches/{match.Id}");
	}

}