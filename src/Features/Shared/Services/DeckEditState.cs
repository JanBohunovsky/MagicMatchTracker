using MagicMatchTracker.Features.Players.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Shared.Services;

public sealed class DeckEditState(Database database) : EditDialogStateBase<DeckEditModel, Deck>
{
	public bool IsNew { get; private set; }

	protected override DeckEditModel CreateEditModel(Deck entity)
	{
		IsNew = entity.Id == Guid.Empty;
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