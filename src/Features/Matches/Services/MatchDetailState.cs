using System.Diagnostics.CodeAnalysis;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDetailState(
	Database database,
	MatchPlayerSelectionDialogState playerSelectionDialogState,
	MatchEditDialogState editDialogState,
	MatchDeckSelectionDialogState deckSelectionDialogState,
	MatchParticipationEditDialogState participationEditDialogState,
	MatchEventEditDialogState eventEditDialogState,
	MatchCreationHelper matchCreationHelper) : StateBase
{
	public bool IsLoading { get; private set; }
	public Match? Match { get; private set; }

	[MemberNotNullWhen(true, nameof(Match))]
	public bool CanStartMatch => Match is not null
		&& !Match.HasStarted
		&& Match.Participations.Count > 0
		&& Match.Participations.All(p => p.Deck is not null);

	public async Task LoadMatchAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Match?.Id == id)
			return;

		ExecuteWithStateChange(() => IsLoading = true);

		Match = await database.Matches
			.Include(m => m.Participations)
			.ThenInclude(mp => mp.Events)
			.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

		ExecuteWithStateChange(() => IsLoading = false);
	}

	public async Task SelectPlayersAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await playerSelectionDialogState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditMatchAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await editDialogState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task SelectDeckAsync(MatchParticipation participation, CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await deckSelectionDialogState.ShowDialogAsync(participation, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditParticipationAsync(MatchParticipation participation, CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await participationEditDialogState.ShowDialogAsync(participation, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task AddEventAsync(MatchParticipation participation, MatchEventType eventType, CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var matchEvent = new MatchEvent
		{
			Participation = participation,
			Type = eventType,
		};
		var success = await eventEditDialogState.ShowDialogAsync(matchEvent, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditEventAsync(MatchEvent matchEvent, CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await eventEditDialogState.ShowDialogAsync(matchEvent, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task StartMatchAsync(CancellationToken cancellationToken = default)
	{
		if (!CanStartMatch)
			return;

		IsBusy = true;

		var now = DateTimeOffset.Now;
		var today = now.ToDateOnly();

		if (Match.CreatedAt.ToDateOnly() < today)
		{
			var matchNumber = await matchCreationHelper.GetNextMatchNumberAsync(today, cancellationToken);
			Match.MatchNumber = matchNumber;
		}

		Match.TimeStarted = now;
		await database.SaveChangesAsync(cancellationToken);

		IsBusy = false;
	}
}