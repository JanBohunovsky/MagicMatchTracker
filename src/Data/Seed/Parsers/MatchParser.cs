using System.Data;
using MagicMatchTracker.Data.Seed.Extensions;
using Microsoft.Data.Sqlite;

namespace MagicMatchTracker.Data.Seed.Parsers;

public sealed class MatchParser
{
	public async Task<Match> ParseAsync(SqliteDataReader reader, CancellationToken cancellationToken = default)
	{
		var rawDate = await reader.GetFieldValueAsync<long>("Date", cancellationToken);
		var matchNumber = await reader.GetFieldValueAsync<int>("Game", cancellationToken);
		var notes = await reader.GetFieldValueAsync<string>("Notes", cancellationToken);
		var rawTimeStarted = await reader.GetFieldValueOrDefaultAsync<long?>("Time_Started", cancellationToken: cancellationToken);
		var rawTimeEnded = await reader.GetFieldValueOrDefaultAsync<long?>("Time_Ended", cancellationToken: cancellationToken);

		var date = DateTimeOffset.FromUnixTimeSeconds(rawDate).ToDateOnly();
		var timeStarted = rawTimeStarted.HasValue ? DateTimeOffset.FromUnixTimeSeconds(rawTimeStarted.Value) : (DateTimeOffset?)null;
		var timeEnded = rawTimeEnded.HasValue ? DateTimeOffset.FromUnixTimeSeconds(rawTimeEnded.Value) : (DateTimeOffset?)null;

		if (timeStarted.HasValue && date != timeStarted.Value.ToDateOnly())
			throw new FormatException($"Inconsistent date: Date={date} vs TimeStarted={timeStarted.Value.ToDateOnly()}");
		if (timeEnded.HasValue && date != timeEnded.Value.ToDateOnly())
			throw new FormatException($"Inconsistent date: Date={date} vs TimeEnded={timeEnded.Value.ToDateOnly()}");
		if (timeStarted.HasValue != timeEnded.HasValue)
			throw new FormatException($"Invalid match state: TimeStarted={timeStarted} vs TimeEnded={timeEnded}");

		var isLive = true;
		if (timeStarted is null || timeEnded is null)
		{
			timeStarted = Match.GetDateTimeForNonLiveMatch(date);
			timeEnded = Match.GetDateTimeForNonLiveMatch(date);
			isLive = false;
		}

		return new Match
		{
			TimeStarted = timeStarted.Value.ToLocalTime(),
			TimeEnded = timeEnded.Value.ToLocalTime(),
			MatchNumber = matchNumber,
			IsLive = isLive,
			Notes = notes.TrimToNull(),
		};
	}
}