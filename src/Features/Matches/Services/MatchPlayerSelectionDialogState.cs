using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchPlayerSelectionDialogState(Database database) : EditDialogStateBase<MatchPlayerSelectModel, Match>
{
	public bool IsNew { get; private set; }

	protected override MatchPlayerSelectModel CreateEditModel(Match entity)
	{
		IsNew = entity.Id == Guid.Empty;
		return new MatchPlayerSelectModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchPlayerSelectModel model, CancellationToken cancellationToken)
	{
		var match = model.ApplyChanges();
		if (IsNew)
			database.Matches.Add(match);

		await database.SaveChangesAsync(cancellationToken);
	}
}