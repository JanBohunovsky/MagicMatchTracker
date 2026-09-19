using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Dialogs.EndTransition;

public sealed class MatchEndTransitionDialogState(Database database) : EditDialogStateBase<MatchEndTransitionModel, Match>
{
	protected override MatchEndTransitionModel CreateEditModel(Match entity)
	{
		if (!entity.HasStarted)
			throw new ArgumentException("The match hasn't started yet.", nameof(entity));
		if (entity.HasEnded)
			throw new ArgumentException("The match has already ended.", nameof(entity));

		return new MatchEndTransitionModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchEndTransitionModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}