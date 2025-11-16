using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDetailState(
	Database database,
	MatchListingState listingState,
	MatchPlayerSelectionDialogState playerSelectionDialogState,
	MatchEditDialogState editDialogState,
	MatchDeckSelectionDialogState deckSelectionDialogState,
	MatchParticipationDetailsEditDialogState participationDetailsEditDialogState,
	MatchParticipationEndStateEditDialogState participationEndStateEditDialogState,
	MatchCreationHelper matchCreationHelper) : StateBase
{
	public bool IsLoading { get; private set; }
	public Match? Match { get; private set; }
	public bool ShowErrors { get; private set; }

	public IEnumerable<MatchError> GetErrors()
	{
		if (Match is null || Match.Participations.Count == 0)
			yield break;

		var missingDecks = Match.Participations
			.Where(mp => mp.Deck is null)
			.ToList();
		if (missingDecks.Count > 0)
			yield return new MatchError("Each player must have a deck selected", missingDecks);

		var winners = Match.Participations
			.Where(mp => mp.EndState?.IsWinner is true)
			.ToList();
		if (winners.Count > 1)
			yield return new MatchError("Only one player can win a match", winners);
	}

	public async Task LoadMatchAsync(Guid id, CancellationToken cancellationToken = default)
	{
		ShowErrors = false;
		if (Match?.Id == id)
			return;

		ExecuteWithStateChange(() => IsLoading = true);

		Match = await database.Matches
			.Include(m => m.Participations)
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

	public async Task EditParticipationDetailsAsync(MatchParticipation participation, CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await participationDetailsEditDialogState.ShowDialogAsync(participation, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditParticipationEndStateAsync(MatchParticipation participation, CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await participationEndStateEditDialogState.ShowDialogAsync(participation, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task StartMatchAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null || Match.HasStarted)
			return;

		if (GetErrors().Any())
		{
			ShowErrors = true;
			return;
		}

		ShowErrors = false;
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

	public async Task EndMatchAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null || !Match.HasStarted || Match.HasEnded)
			return;

		if (GetErrors().Any())
		{
			ShowErrors = true;
			return;
		}

		ShowErrors = false;
		IsBusy = true;

		Match.TimeEnded = DateTimeOffset.Now;
		await database.SaveChangesAsync(cancellationToken);

		IsBusy = false;
	}

	public async Task PlayAgainAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null || !Match.HasEnded)
			return;

		IsBusy = true;

		await listingState.LoadMatchesAsync(cancellationToken);
		await listingState.AddNewMatchAsync(Match, cancellationToken);

		IsBusy = false;
	}
}