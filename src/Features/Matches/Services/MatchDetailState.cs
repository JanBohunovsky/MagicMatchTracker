using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDetailState(
	Database database,
	MatchPlayerSelectionState playerSelectionState,
	MatchEditState editState,
	MatchDeckSelectionState deckSelectionState) : StateBase
{
	public Match? Match { get; private set; }

	public async Task LoadMatchAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Match?.Id == id)
			return;

		await WithBusyAsync(async () =>
		{
			Match = await database.Matches
				.Include(m => m.Participations)
				.ThenInclude(mp => mp.Events)
				.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
		});
	}

	public async Task SelectPlayersAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await playerSelectionState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditMatchAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await editState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task SelectDeckAsync(MatchParticipation participation, CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await deckSelectionState.ShowDialogAsync(participation, cancellationToken);
		if (success)
			NotifyStateChanged();
	}
}