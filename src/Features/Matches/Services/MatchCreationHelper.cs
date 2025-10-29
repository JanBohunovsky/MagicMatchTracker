using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchCreationHelper(Database database)
{
	public async Task<int> GetNextMatchNumberAsync(DateOnly date, CancellationToken cancellationToken = default)
	{
		var lastMatchNumber = await database.Matches
			.Where(m => m.TimeStarted == null
				? DateOnly.FromDateTime(m.CreatedAt.Date) == date
				: DateOnly.FromDateTime(m.TimeStarted.Value.Date) == date)
			.Select(m => m.MatchNumber)
			.OrderDescending()
			.FirstOrDefaultAsync(cancellationToken);

		return lastMatchNumber + 1;
	}
}