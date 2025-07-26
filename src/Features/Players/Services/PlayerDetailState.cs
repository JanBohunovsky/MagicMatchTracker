using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Players.Services;

public sealed class PlayerDetailState(Database database, PlayerEditState playerEditState, DeckEditState deckEditState) : StateBase
{
	public Player? Player { get; private set; }

	public async Task LoadPlayerAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Player?.Id == id)
			return;

		await WithBusyAsync(async () =>
		{
			Player = await database.Players
				.Include(p => p.Decks)
				.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		});
	}

	public async Task EditPlayerAsync(CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var success = await playerEditState.ShowDialogAsync(Player, cancellationToken);
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
		var success = await deckEditState.ShowDialogAsync(deck, cancellationToken);
		if (success)
			NotifyStateChanged();
	}
}