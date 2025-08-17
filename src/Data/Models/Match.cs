using System.Text;

namespace MagicMatchTracker.Data.Models;

public sealed class Match : IEntity
{
	public Guid Id { get; private init; }

	public DateTimeOffset? TimeStarted { get; set; }

	public DateTimeOffset? TimeEnded { get; set; }

	public int MatchNumber { get; set; }

	public string? Notes { get; set; }

	public List<MatchParticipation> Participations { get; set; } = [];

	public DateTimeOffset CreatedAt { get; private init; } = DateTimeOffset.Now;

	public DateOnly GetEffectiveDate() => TimeStarted?.Date.ToDateOnly() ?? CreatedAt.Date.ToDateOnly();

	public string GetTitle(bool includeDate = true)
	{
		var sb = new StringBuilder();
		if (TimeStarted is null)
			sb.Append("Draft ");

		sb.Append($"Match #{MatchNumber}");

		if (!includeDate)
			return sb.ToString();

		sb.Append(" - ");

		var date = GetEffectiveDate();
		if (date.Year < DateTimeOffset.Now.Year)
		{
			sb.Append(date.ToString("dd MMMM yyyy"));
		}
		else
		{
			sb.Append(date.ToString("dddd, dd MMMM"));
		}

		return sb.ToString();
	}
}