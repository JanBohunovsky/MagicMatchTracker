using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Features.Shared.Dialogs.DeckEdit;
using MagicMatchTracker.Features.Shared.Extensions;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Players.Pages.Detail;

public sealed class PlayerDetailState(Database database, PlayerEditDialogState playerEditDialogState, DeckEditDialogState deckEditDialogState) : StateBase
{
	public Player? Player { get; private set; }
	public IReadOnlyDictionary<Guid, Stats> DeckStats { get; private set; } = new Dictionary<Guid, Stats>();

	public IEnumerable<Deck> ActiveDecks => Player is not null
		? Player.Decks
			.Where(d => !d.IsArchived)
			.OrderBy(d => d.Name ?? d.Commander)
		: [];

	public IEnumerable<Deck> ArchivedDecks => Player is not null
		? Player.Decks
			.Where(d => d.IsArchived)
			.OrderBy(d => d.Name ?? d.Commander)
		: [];

	public async Task LoadPlayerAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Player?.Id == id)
			return;

		IsBusy = true;

		Player = await database.Players
			.Include(p => p.Decks)
			.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

		IsBusy = false;
	}

	public async Task LoadDeckStatsAsync(CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var playerDecks = Player.Decks.Select(d => d.Id);
		DeckStats = await database.QueryDeckStats()
			.Where(d => playerDecks.Contains(d.DeckId))
			.ToDictionaryAsync(d => d.DeckId, d => (Stats)d, cancellationToken);
	}

	public async Task EditPlayerAsync(CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var success = await playerEditDialogState.ShowDialogAsync(Player, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task AddNewDeckAsync(CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var deck = new Deck
		{
			Owner = Player,
			Commander = "",
		};
		var success = await deckEditDialogState.ShowDialogAsync(deck, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditDeckAsync(Deck deck, CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var success = await deckEditDialogState.ShowDialogAsync(deck, cancellationToken);
		if (success)
			NotifyStateChanged();
	}
}