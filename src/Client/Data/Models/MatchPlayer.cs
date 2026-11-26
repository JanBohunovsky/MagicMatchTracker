using System.Text;
using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class MatchPlayer
{
	[JsonIgnore]
	public Match Match { get; set; } = null!;

	public required string PlayerId { get; set; }

	[JsonIgnore]
	public Player Player { get; set; } = null!;

	public required string DeckId { get; set; }

	[JsonIgnore]
	public Deck Deck { get; set; } = null!;

	public int TurnOrder { get; set; }

	public int? EndTurn { get; set; }

	public DateTimeOffset? EndTime { get; set; }

	public TimeSpan? Duration => EndTime is null || Match.StartTime is null ? null : EndTime.Value - Match.StartTime.Value;

	public bool IsWinner { get; set; }

	public MatchElimination? Elimination { get; set; }

	public string BuildDescription(bool includeTurn = true)
	{
		var sb = new StringBuilder();

		if (IsWinner)
		{
			sb.Append("Won");
		}
		else if (Elimination is not null)
		{
			sb.Append(Elimination.Method.DisplayName);
		}
		else
		{
			sb.Append("Eliminated");
		}

		if (Elimination is not null && Elimination.Method is not EliminationMethod.Concede)
		{
			sb.Append($" by {Elimination.Killer.Username}");
		}

		if (includeTurn && EndTurn is not null)
		{
			sb.Append($" on turn {EndTurn}");
		}

		return sb.ToString();
	}
}