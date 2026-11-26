using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class Match
{
	public string Id { get; private set; } = string.Empty;

	[JsonIgnore]
	public Session Session { get; set; } = null!;

	public required int Number
	{
		get;
		set
		{
			field = value;
			Id = Number.ToString();
		}
	}

	public DateTimeOffset? StartTime { get; set; }
	public DateTimeOffset? EndTime { get; set; }

	public TimeSpan? Duration => StartTime is null || EndTime is null ? null : EndTime - StartTime;

	public List<MatchPlayer> Players { get; set; } = [];

	public int? GetTotalTurns() => Players.Max(p => p.EndTurn);
}