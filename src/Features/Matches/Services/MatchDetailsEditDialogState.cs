using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDetailsEditDialogState(Database database) : EditDialogStateBase<MatchDetailsEditModel, Match>
{
	protected override MatchDetailsEditModel CreateEditModel(Match entity)
	{
		if (entity.Id == Guid.Empty)
			throw new InvalidOperationException($"{nameof(MatchDetailsEditDialogState)} cannot be used for new entities.");

		return new MatchDetailsEditModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchDetailsEditModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}