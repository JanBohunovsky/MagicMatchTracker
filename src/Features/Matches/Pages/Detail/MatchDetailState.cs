using MagicMatchTracker.Features.Matches.Dialogs.DeckSelect;
using MagicMatchTracker.Features.Matches.Dialogs.DetailsEdit;
using MagicMatchTracker.Features.Matches.Dialogs.EndTransition;
using MagicMatchTracker.Features.Matches.Dialogs.ParticipationDetailsEdit;
using MagicMatchTracker.Features.Matches.Dialogs.ParticipationEndStateEdit;
using MagicMatchTracker.Features.Matches.Dialogs.PlayersEdit;
using MagicMatchTracker.Features.Matches.Dialogs.StartTransition;
using MagicMatchTracker.Features.Matches.Events;
using MagicMatchTracker.Features.Matches.Services;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Pages.Detail;

public sealed class MatchDetailState(
	Database database,
	MatchNumberingService matchNumberingService,
	NavigationManager navigationManager,
	IMessageHub messageHub,
	MatchPlayersEditDialogState playersEditDialogState,
	MatchDetailsEditDialogState detailsEditDialogState,
	MatchDeckSelectionDialogState deckSelectionDialogState,
	MatchParticipationDetailsEditDialogState participationDetailsEditDialogState,
	MatchParticipationEndStateEditDialogState participationEndStateEditDialogState,
	MatchStartTransitionDialogState startTransitionDialogState,
	MatchEndTransitionDialogState endTransitionDialogState) : StateBase
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

		var success = await playersEditDialogState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditMatchAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null)
			return;

		var success = await detailsEditDialogState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task DeleteMatchAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null || Match.Participations.Count > 0)
			return;

		database.Matches.Remove(Match);
		await database.SaveChangesAsync(cancellationToken);
		messageHub.Publish(new MatchDeletedEvent(Match));

		navigationManager.NavigateTo("/matches");
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

		var success = await startTransitionDialogState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
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

		var success = await endTransitionDialogState.ShowDialogAsync(Match, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task PlayAgainAsync(CancellationToken cancellationToken = default)
	{
		if (Match is null || !Match.HasEnded)
			return;

		IsBusy = true;

		var match = new Match
		{
			MatchNumber = await matchNumberingService.GetNextMatchNumberAsync(DateOnly.Today, cancellationToken),
		};

		foreach (var participation in Match.Participations)
		{
			match.Participations.Add(new MatchParticipation
			{
				Match = match,
				Player = participation.Player,
				Deck = participation.Deck,
			});
		}

		IsBusy = false;

		var success = await playersEditDialogState.ShowDialogAsync(match, cancellationToken);
		if (!success)
			return;

		navigationManager.NavigateTo($"/matches/{match.Id}");
	}
}