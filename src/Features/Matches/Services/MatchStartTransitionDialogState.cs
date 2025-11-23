using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchStartTransitionDialogState(Database database) : EditDialogStateBase<MatchStartTransitionModel, Match>
{
	protected override MatchStartTransitionModel CreateEditModel(Match entity)
	{
		if (entity.HasStarted)
			throw new ArgumentException("The match has already started.", nameof(entity));

		return new MatchStartTransitionModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchStartTransitionModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}