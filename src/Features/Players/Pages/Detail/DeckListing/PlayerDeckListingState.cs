using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Features.Shared.Dialogs.DeckEdit;
using MagicMatchTracker.Features.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Players.Pages.Detail.DeckListing;

public sealed class PlayerDeckListingState(
	Database database,
	DeckEditDialogState deckEditDialogState,
	PlayerEditDialogState playerEditDialogState) : PlayerDetailStateBase(playerEditDialogState)
{
	public IReadOnlyDictionary<Guid, Stats> DeckStats { get; private set; } = new Dictionary<Guid, Stats>();

	protected override async Task<Player?> LoadPlayerCoreAsync(Guid id, CancellationToken cancellationToken)
	{
		return await database.Players
			.Include(p => p.Decks)
			.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
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