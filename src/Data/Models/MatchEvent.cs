namespace MagicMatchTracker.Data.Models;

public sealed class MatchEvent : IEntity
{
	public Guid Id { get; private init; }

	public required MatchParticipation Participation { get; set; }

	public required int Turn { get; set; }

	public DateTimeOffset Time { get; private init; } = DateTimeOffset.Now;

	public MatchEventType Type { get; set; }

	public MatchEventData? Data { get; set; }
}