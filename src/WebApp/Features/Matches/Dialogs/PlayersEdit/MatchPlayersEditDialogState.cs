using MagicMatchTracker.Features.Matches.Events;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Dialogs.PlayersEdit;

public sealed class MatchPlayersEditDialogState(Database database, IMessageHub messageHub) : EditDialogStateBase<MatchPlayersEditModel, Match>
{
	public bool IsNew { get; private set; }

	protected override MatchPlayersEditModel CreateEditModel(Match entity)
	{
		IsNew = entity.Id == Guid.Empty;
		return new MatchPlayersEditModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchPlayersEditModel model, CancellationToken cancellationToken)
	{
		var match = model.ApplyChanges();
		if (IsNew)
			database.Matches.Add(match);

		await database.SaveChangesAsync(cancellationToken);

		if (IsNew)
			messageHub.Publish(new MatchCreatedEvent(match));
	}
}