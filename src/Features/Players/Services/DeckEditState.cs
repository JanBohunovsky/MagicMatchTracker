using MagicMatchTracker.Features.Players.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Players.Services;

public sealed class DeckEditState(Database database) : EditDialogStateBase<DeckEditModel, Deck>
{
	protected override DeckEditModel CreateEditModel(Deck entity)
	{
		return new DeckEditModel(entity);
	}

	protected override async Task SaveCoreAsync(DeckEditModel model, CancellationToken cancellationToken)
	{
		var deck = model.ApplyChanges();
		if (IsNew)
			database.Decks.Add(deck);

		await database.SaveChangesAsync(cancellationToken);
	}
}