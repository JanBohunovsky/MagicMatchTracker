using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchListingState(Database database, MatchPlayerSelectionState playerSelectionState, NavigationManager navigationManager) : StateBase
{
	private List<Match>? _matches;

	public IReadOnlyList<Match>? Matches => _matches;

	public async Task LoadMatchesAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is not null)
			return;

		_matches = await database.Matches
			.OrderByDescending(m => m.TimeStarted ?? m.CreatedAt)
			.ToListAsync(cancellationToken: cancellationToken);
	}

	public async Task AddNewMatchAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is null)
			return;

		var today = DateTimeOffset.Now.ToDateOnly();
		var lastMatchNumberOfToday = _matches.Where(m => m.GetEffectiveDate() == today)
			.Select(m => m.MatchNumber)
			.DefaultIfEmpty(0)
			.Max();

		var match = new Match
		{
			MatchNumber = lastMatchNumberOfToday + 1,
		};

		var success = await playerSelectionState.ShowDialogAndWaitAsync(match, cancellationToken);
		if (!success)
			return;

		_matches.Insert(0, match);
		navigationManager.NavigateTo($"/matches/{match.Id}");
	}

}