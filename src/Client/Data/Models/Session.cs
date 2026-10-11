using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class Session
{
	public string Slug { get; private set; } = string.Empty;

	public required DateOnly Date
	{
		get;
		set
		{
			field = value;
			Slug = value.ToString("yyyy-MM-dd");
		}
	}

	// List of players that were attending this session. Also serves as a seating order.
	public required string[] SeatedPlayerSlugs { get; set; }

	public List<Match> Matches { get; set; } = [];

	[JsonIgnore]
	public Player[] SeatedPlayers
	{
		get;
		set
		{
			field = value;
			SeatedPlayerSlugs = value.Select(player => player.Slug).ToArray();
		}
	}

	public string FormattedDate => Date.ToString("MMMM d, dddd");

	public string? GetFormattedDuration()
	{
		if (Matches.Count == 0)
			return null;

		var lastMatch = Matches.MaxBy(m => m.Number);
		if (lastMatch?.EndTime is null)
			return null;

		var firstMatch = Matches.MinBy(m => m.Number);
		if (firstMatch?.StartTime is null)
			return null;

		var duration = lastMatch.EndTime.Value - firstMatch.StartTime.Value;
		return duration.ToShortString();
	}
}