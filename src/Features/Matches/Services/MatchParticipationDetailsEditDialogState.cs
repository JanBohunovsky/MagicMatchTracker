using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchParticipationDetailsEditDialogState(Database database) : EditDialogStateBase<MatchParticipationDetailsEditModel, MatchParticipation>
{
	protected override MatchParticipationDetailsEditModel CreateEditModel(MatchParticipation entity)
	{
		return new MatchParticipationDetailsEditModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchParticipationDetailsEditModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}