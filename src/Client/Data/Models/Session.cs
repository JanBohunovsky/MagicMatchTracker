using System.Globalization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class Session
{
	public string Id { get; private set; } = string.Empty;

	public required DateOnly Date
	{
		get;
		set
		{
			field = value;
			Id = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		}
	}

	public string? Name { get; set; }

	public string? Location { get; set; }

	public required List<SessionSeat> Seats { get; set; }

	public List<Match> Matches { get; set; } = [];

	public string FormattedDate => Date.ToString("MMMM d, dddd", CultureInfo.InvariantCulture);
}