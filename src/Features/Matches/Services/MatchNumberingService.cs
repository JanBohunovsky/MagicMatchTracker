using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchNumberingService(Database database)
{
	public async Task<int> GetNextMatchNumberAsync(DateOnly date, CancellationToken cancellationToken = default)
	{
		var today = DateOnly.Today;
		var lastMatchNumber = await database.Matches
			.Where(m => m.TimeStarted != null
				? DateOnly.FromDateTime(m.TimeStarted.Value.Date) == date
				: today == date)
			.Select(m => m.MatchNumber)
			.OrderDescending()
			.FirstOrDefaultAsync(cancellationToken);

		return lastMatchNumber + 1;
	}
}