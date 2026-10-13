using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class SessionPlayer
{
	public required string PlayerSlug { get; set; }
	public required int SeatNumber { get; set; }

	[JsonIgnore]
	public Player Player { get; set; } = null!;
}