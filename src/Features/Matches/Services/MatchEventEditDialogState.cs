using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchEventEditDialogState(Database database) : EditDialogStateBase<MatchEventEditModel, MatchEvent>
{
	public bool IsNew { get; private set; }

	protected override MatchEventEditModel CreateEditModel(MatchEvent entity)
	{
		IsNew = entity.Id == Guid.Empty;
		return new MatchEventEditModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchEventEditModel model, CancellationToken cancellationToken)
	{
		var matchEvent = model.ApplyChanges();
		if (IsNew)
			database.MatchEvents.Add(matchEvent);

		await database.SaveChangesAsync(cancellationToken);
	}
}