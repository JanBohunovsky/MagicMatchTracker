using MagicMatchTracker.Features.Players.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Players.Services;

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

	public async Task ToggleArchiveAsync(CancellationToken cancellationToken)
	{
		if (Model is null || IsNew)
			return;

		await WithBusyAsync(async () =>
		{
			Model.ToggleArchiveState();
			await database.SaveChangesAsync(cancellationToken);
		});

		HideDialog(success: true);
	}
}