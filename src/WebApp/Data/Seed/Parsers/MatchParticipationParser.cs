using System.Data;
using MagicMatchTracker.Data.Seed.Extensions;
using Microsoft.Data.Sqlite;

namespace MagicMatchTracker.Data.Seed.Parsers;

public sealed class MatchParticipationParser(IReadOnlyGristData data)
{
	private readonly LoseConditionParser _loseConditionParser = new();

	public async Task<MatchParticipation> ParseAsync(SqliteDataReader reader, CancellationToken cancellationToken = default)
	{
		var matchId = await reader.GetFieldValueAsync<int>("Match", cancellationToken);
		var playerId = await reader.GetFieldValueAsync<int>("Player", cancellationToken);
		var deckId = await reader.GetFieldValueAsync<int>("Deck", cancellationToken);
		var isWinner = await reader.GetFieldValueAsync<bool>("Winner", cancellationToken);
		var notes = await reader.GetFieldValueAsync<string>("Notes", cancellationToken);
		var killerId = await reader.GetFieldValueAsync<int>("Eliminated_by", cancellationToken);
		var loseConditionId = await reader.GetFieldValueAsync<int>("Cause_of_loss", cancellationToken);
		var rawEndTime = await reader.GetFieldValueOrDefaultAsync<long?>("Time_of_loss", cancellationToken: cancellationToken);
		var endTurn = await reader.GetFieldValueOrDefaultAsync<int?>("Turn_lost", cancellationToken: cancellationToken);
		var turnOrder = await reader.GetFieldValueOrDefaultAsync<int?>("Turn_order", cancellationToken: cancellationToken);

		var loseCondition = _loseConditionParser.Parse(loseConditionId);
		var endTime = rawEndTime.HasValue ? DateTimeOffset.FromUnixTimeSeconds(rawEndTime.Value) : (DateTimeOffset?)null;

		return new MatchParticipation
		{
			Match = data.Matches[matchId],
			Player = data.Players[playerId],
			Deck = data.Decks[deckId],
			TurnOrder = turnOrder,
			Notes = notes.TrimToNull(),
			EndState = new MatchParticipationEndState
			{
				IsWinner = isWinner,
				Turn = endTurn,
				Time = endTime?.ToLocalTime(),
				LoseCondition = loseCondition,
				Killer = killerId > 0 ? data.Players[killerId] : null,
			},
		};
	}
}