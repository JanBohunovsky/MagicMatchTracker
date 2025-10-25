using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDeckSelectionState(Database database) : EditDialogStateBase<MatchDeckSelectModel, MatchParticipation>
{
	protected override MatchDeckSelectModel CreateEditModel(MatchParticipation entity)
	{
		return new MatchDeckSelectModel(entity);
	}

	protected override async Task SaveCoreAsync(MatchDeckSelectModel model, CancellationToken cancellationToken)
	{
		model.ApplyChanges();
		await database.SaveChangesAsync(cancellationToken);
	}
}