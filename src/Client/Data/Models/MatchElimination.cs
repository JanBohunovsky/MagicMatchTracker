using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class MatchElimination
{
	public required string KillerId { get; set; }

	[JsonIgnore]
	public Player Killer { get; set; } = null!;

	public required EliminationMethod Method { get; set; }
}