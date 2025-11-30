using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Shared.Extensions;

public static class DatabaseStatsExtensions
{
	private const string PlayerIdColumn = "player_id";
	private const string DeckIdColumn = "deck_id";

	private const string QueryTemplate =
		"""
		select mp.{0},
			   count(*)                                                        as matches,
			   count(*) filter (where mp.end_state_is_winner)                  as wins,
			   max(date_trunc('day', m.time_started at time zone 'utc')::date) as last_played
		from match_participations mp
				 inner join matches m on mp.match_id = m.id
		where m.time_ended is not null
		group by mp.{0}
		""";

	extension(Database database)
	{
		public IQueryable<PlayerStats> QueryPlayerStats()
		{
			var query = string.Format(QueryTemplate, PlayerIdColumn);

			return database.Database.SqlQueryRaw<PlayerStats>(query);
		}

		public IQueryable<DeckStats> QueryDeckStats()
		{
			var query = string.Format(QueryTemplate, DeckIdColumn);

			return database.Database.SqlQueryRaw<DeckStats>(query);
		}
	}
}