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

	public required List<SessionPlayer> Players { get; set; }

	public List<Match> Matches { get; set; } = [];

	public string FormattedDate => Date.ToString("MMMM d, dddd");

	public string? GetFormattedDuration()
	{
		if (Matches.Count == 0)
			return null;

		var lastMatch = Matches.Where(m => m.IsLive).MaxBy(m => m.Number);
		if (lastMatch?.EndTime is null)
			return null;

		var firstMatch = Matches.Where(m => m.IsLive).MinBy(m => m.Number);
		if (firstMatch?.StartTime is null)
			return null;

		var duration = lastMatch.EndTime.Value - firstMatch.StartTime.Value;
		return duration.ToShortString();
	}
}