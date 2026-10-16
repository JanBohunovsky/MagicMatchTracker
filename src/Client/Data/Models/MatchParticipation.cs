using System.Text;
using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class MatchParticipation
{
	public required string PlayerSlug { get; set; }
	public required string DeckSlug { get; set; }
	public int TurnOrder { get; set; }
	public bool IsWinner { get; set; }
	public DateTimeOffset? EndTime { get; set; }
	public int? EndTurn { get; set; }
	public string? EliminatedByPlayerSlug { get; set; }
	public EliminationReason? EliminationReason { get; set; }

	[JsonIgnore]
	public Match Match { get; set; } = null!;

	[JsonIgnore]
	public Player Player { get; set; } = null!;

	[JsonIgnore]
	public Deck Deck { get; set; } = null!;

	[JsonIgnore]
	public Player? EliminatedBy { get; set; }

	public string BuildDescription()
	{
		var sb = new StringBuilder();

		if (IsWinner)
		{
			sb.Append("Won");
		}
		else if (EliminationReason is not null)
		{
			sb.Append(EliminationReason.Value.DisplayName);
		}
		else if (EliminatedBy is not null)
		{
			sb.Append("Eliminated by");
		}
		else
		{
			sb.Append("Lost");
		}

		if (EliminatedBy is not null)
		{
			sb.Append($" by {EliminatedBy.Username}");
		}

		if (EndTurn is not null)
		{
			sb.Append($" on turn {EndTurn}");
		}

		return sb.ToString();
	}
}