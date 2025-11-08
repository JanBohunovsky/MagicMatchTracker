using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchParticipationEditDialogState(Database database) : EditDialogStateBase<MatchParticipationEditModel, MatchParticipation>
{
	protected override MatchParticipationEditModel CreateEditModel(MatchParticipation entity)
	{
		return new MatchParticipationEditModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchParticipationEditModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}