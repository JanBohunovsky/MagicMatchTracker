using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class SessionSeat
{
	public required string PlayerId { get; set; }

	[JsonIgnore]
	public Player Player { get; set; } = null!;

	public required int SeatNumber { get; set; }

}