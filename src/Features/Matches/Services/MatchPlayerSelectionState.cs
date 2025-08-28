using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchPlayerSelectionState(Database database) : EditDialogStateBase<MatchPlayerSelectModel, Match>
{
	protected override MatchPlayerSelectModel CreateEditModel(Match entity)
	{
		return new MatchPlayerSelectModel(entity);
	}

	protected override async Task InitialiseAsync(Match entity, CancellationToken cancellationToken)
	{
		var players = await database.Players
			.Include(p => p.Decks)
			.Where(p => p.Decks.Count > 0)
			.OrderBy(p => p.Name)
			.ToListAsync(cancellationToken);

		Model?.SetAvailablePlayers(players);
	}

	protected override async Task SaveCoreAsync(MatchPlayerSelectModel model, CancellationToken cancellationToken)
	{
		var match = model.ApplyChanges();
		if (IsNew)
			database.Matches.Add(match);

		await database.SaveChangesAsync(cancellationToken);
	}
}