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
}