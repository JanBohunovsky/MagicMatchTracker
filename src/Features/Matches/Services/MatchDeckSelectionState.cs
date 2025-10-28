using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Features.Shared.Services;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDeckSelectionState(Database database, DeckEditState deckEditState) : EditDialogStateBase<MatchDeckSelectModel, MatchParticipation>
{
	protected override MatchDeckSelectModel CreateEditModel(MatchParticipation entity)
	{
		return new MatchDeckSelectModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchDeckSelectModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> AddNewDeckAsync(Player player, CancellationToken cancellationToken = default)
	{
		var deck = new Deck
		{
			Owner = player,
			Commander = string.Empty,
		};
		return await deckEditState.ShowDialogAsync(deck, cancellationToken);
	}

	public async Task<bool> EditDeckAsync(Deck deck, CancellationToken cancellationToken = default)
	{
		return await deckEditState.ShowDialogAsync(deck, cancellationToken);
	}
}