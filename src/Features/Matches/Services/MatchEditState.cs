using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchEditState(Database database) : EditDialogStateBase<MatchEditModel, Match>
{
	protected override MatchEditModel CreateEditModel(Match entity)
	{
		if (IsNew)
			throw new InvalidOperationException($"{nameof(MatchEditState)} cannot be used for new entities.");

		return new MatchEditModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchEditModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}