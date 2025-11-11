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
	/// <summary>
	/// The ID of a player from the same match who caused this player to lose.
	/// </summary>
	public required Guid KillerId { get; set; }

	public required LoseCondition LoseCondition { get; set; }
}