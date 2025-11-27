using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Players.Dialogs.Edit;

public sealed class PlayerEditDialogState(Database database) : EditDialogStateBase<PlayerEditModel, Player>
{
	public bool IsNew { get; private set; }

	protected override PlayerEditModel CreateEditModel(Player entity)
	{
		IsNew = entity.Id == Guid.Empty;
		return new PlayerEditModel(entity);
	}

	protected override async Task SaveCoreAsync(PlayerEditModel model, CancellationToken cancellationToken)
	{
		var player = model.ApplyChanges();
		if (IsNew)
			database.Players.Add(player);

		await database.SaveChangesAsync(cancellationToken);
	}
}