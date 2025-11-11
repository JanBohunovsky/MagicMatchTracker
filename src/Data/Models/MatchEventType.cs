namespace MagicMatchTracker.Data.Models;

public enum MatchEventType
{
	PlayerLost,
	PlayerWon,
}

public static class MatchEventTypeExtensions
{
 	/// <summary>
	/// Whether the event indicates that the player has stopped playing.
	/// </summary>
	public static bool IsTerminal(this MatchEventType type)
		=> type is MatchEventType.PlayerLost or MatchEventType.PlayerWon;

	public static string GetDisplayName(this MatchEventType type) => type switch
	{
		MatchEventType.PlayerLost => "Player lost",
		MatchEventType.PlayerWon => "Player won",
		_ => throw new ArgumentException($"Invalid match event type: {type}", nameof(type)),
	};
}