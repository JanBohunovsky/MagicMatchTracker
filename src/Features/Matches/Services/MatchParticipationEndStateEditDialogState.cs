using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchParticipationEndStateEditDialogState(Database database) : EditDialogStateBase<MatchParticipationEndStateEditModel, MatchParticipation>
{
	public bool IsNew { get; private set; }

	protected override MatchParticipationEndStateEditModel CreateEditModel(MatchParticipation entity)
	{
		IsNew = entity.EndState is null;
		return new MatchParticipationEndStateEditModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchParticipationEndStateEditModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}