using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Extensions;

public static class MatchDbSetExtensions
{
	extension(DbSet<Match> matches)
	{
		public async Task<int> GetNewMatchNumberAsync(DateOnly date, CancellationToken cancellationToken = default)
		{
			var today = DateOnly.Today;
			var lastMatchNumber = await matches
				.Where(m => m.TimeStarted != null
					? DateOnly.FromDateTime(m.TimeStarted.Value.Date) == date
					: today == date)
				.Select(m => m.MatchNumber)
				.OrderDescending()
				.FirstOrDefaultAsync(cancellationToken);

			return lastMatchNumber + 1;
		}

		public IQueryable<Match> QueryNextMatches(Match match)
		{
			var today = DateOnly.Today;
			var date = match.GetEffectiveDate();

			return matches.Select(m => new
				{
					Match = m,
					EffectiveDate = m.TimeStarted != null ? DateOnly.FromDateTime(m.TimeStarted.Value.Date) : today,
				})
				.Where(x => x.EffectiveDate > date || (x.EffectiveDate == date && x.Match.MatchNumber > match.MatchNumber))
				.OrderBy(x => x.EffectiveDate)
				.ThenBy(x => x.Match.MatchNumber)
				.Select(x => x.Match);
		}

		public IQueryable<Match> QueryPreviousMatches(Match match)
		{
			var today = DateOnly.Today;
			var date = match.GetEffectiveDate();

			return matches.Select(m => new
				{
					Match = m,
					EffectiveDate = m.TimeStarted != null ? DateOnly.FromDateTime(m.TimeStarted.Value.Date) : today,
				})
				.Where(x => x.EffectiveDate < date || (x.EffectiveDate == date && x.Match.MatchNumber < match.MatchNumber))
				.OrderByDescending(x => x.EffectiveDate)
				.ThenByDescending(x => x.Match.MatchNumber)
				.Select(x => x.Match);
		}

		public IQueryable<DateOnly> QueryMatchDates()
		{
			var today = DateOnly.Today;

			return matches.Select(m => m.TimeStarted != null ? DateOnly.FromDateTime(m.TimeStarted.Value.Date) : today)
				.Distinct()
				.OrderDescending();
		}
	}
}