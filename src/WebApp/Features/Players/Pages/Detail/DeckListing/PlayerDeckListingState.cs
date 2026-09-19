using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Features.Shared.Dialogs.DeckEdit;
using MagicMatchTracker.Features.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Players.Pages.Detail.DeckListing;

public sealed class PlayerDeckListingState : PlayerDetailStateBase
{
	private readonly DeckEditDialogState _deckEditDialogState;

	public IReadOnlyDictionary<Guid, Stats> DeckStats { get; private set; } = new Dictionary<Guid, Stats>();

	// ReSharper disable once ConvertToPrimaryConstructor - Issue with capturing `database` twice
	public PlayerDeckListingState(
		Database database,
		DeckEditDialogState deckEditDialogState,
		PlayerEditDialogState playerEditDialogState)
		: base(database, playerEditDialogState)
	{
		_deckEditDialogState = deckEditDialogState;
	}

	protected override async Task<Player?> LoadPlayerCoreAsync(Guid id, CancellationToken cancellationToken)
	{
		var player = await Database.Players
			.Include(p => p.Decks)
			.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

		if (player is null)
			return null;

		var playerDecks = player.Decks.Select(d => d.Id);
		DeckStats = await Database.QueryDeckStats()
			.Where(d => playerDecks.Contains(d.DeckId))
			.ToDictionaryAsync(d => d.DeckId, d => (Stats)d, cancellationToken);

		return player;
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
		var success = await _deckEditDialogState.ShowDialogAsync(deck, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	public async Task EditDeckAsync(Deck deck, CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var success = await _deckEditDialogState.ShowDialogAsync(deck, cancellationToken);
		if (success)
			NotifyStateChanged();
	}
}