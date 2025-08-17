using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDetailState(Database database) : StateBase
{
	public Match? Match { get; private set; }

	public async Task LoadMatchAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Match?.Id == id)
			return;

		await WithBusyAsync(async () =>
		{
			Match = await database.Matches
				.Include(m => m.Participations)
				.ThenInclude(mp => mp.Events)
				.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
		});
	}
}