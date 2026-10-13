using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class MatchParticipation
{
	public required string PlayerSlug { get; set; }
	public required string DeckSlug { get; set; }
	public int TurnOrder { get; set; }
	public bool IsWinner { get; set; }

	[JsonIgnore]
	public Match Match { get; set; } = null!;

	[JsonIgnore]
	public Player Player { get; set; } = null!;

	[JsonIgnore]
	public Deck Deck { get; set; } = null!;
}