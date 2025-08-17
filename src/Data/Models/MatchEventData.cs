using System.Text.Json.Serialization;

namespace MagicMatchTracker.Data.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(PlayerLostEventData), nameof(MatchEventType.PlayerLost))]
public class MatchEventData
{
	public string? Notes { get; set; }
}

public sealed class PlayerLostEventData : MatchEventData
{
	public required MatchParticipation Source { get; set; }
	public required LoseCondition LoseCondition { get; set; }
}