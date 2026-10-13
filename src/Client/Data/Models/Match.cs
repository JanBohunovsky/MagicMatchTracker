using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class Match
{
	public string Slug { get; private set; } = string.Empty;

	public required int Number
	{
		get;
		set
		{
			field = value;
			Slug = Number.ToString();
		}
	}

	public DateTimeOffset? StartTime { get; set; }
	public DateTimeOffset? EndTime { get; set; }
	public bool IsLive { get; set; } = true;

	public List<MatchParticipation> Participations { get; set; } = [];

	[JsonIgnore]
	public Session Session { get; set; } = null!;

	public string? GetFormattedDuration()
	{
		if (!IsLive || StartTime is null)
			return null;

		var endTime = EndTime ?? DateTimeOffset.Now;
		var duration = endTime - StartTime.Value;

		return duration.ToShortString();
	}
}