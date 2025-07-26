using MagicMatchTracker.Features.Players.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Players.Services;

public sealed class PlayerEditState(Database database) : EditDialogStateBase<PlayerEditModel, Player>
{
	protected override PlayerEditModel CreateEditModel(Player entity)
	{
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