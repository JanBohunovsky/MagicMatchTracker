using MagicMatchTracker.Features.Shared.Dialogs.DeckEdit;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Dialogs.DeckSelect;

public sealed class MatchDeckSelectionDialogState(Database database, DeckEditDialogState deckEditDialogState) : EditDialogStateBase<MatchDeckSelectModel, MatchParticipation>
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
		return await deckEditDialogState.ShowDialogAsync(deck, cancellationToken);
	}

	public async Task<bool> EditDeckAsync(Deck deck, CancellationToken cancellationToken = default)
	{
		return await deckEditDialogState.ShowDialogAsync(deck, cancellationToken);
	}
}