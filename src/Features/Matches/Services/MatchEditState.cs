using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchEditState(Database database) : StateBase
{
	public MatchEditModel? Match { get; private set; }

	public async Task LoadMatchAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Match?.Id == id)
			return;

		await WithBusyAsync(async () =>
		{
			var match = await database.Matches
				.Include(m => m.Participations)
				.ThenInclude(mp => mp.Events)
				.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

			if (match is null)
				return;

			Match = new MatchEditModel(match);
		});
	}
}