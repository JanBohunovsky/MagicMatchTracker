using System.Diagnostics.CodeAnalysis;
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

	[MemberNotNullWhen(true, nameof(TimeStarted))]
	public bool HasStarted => TimeStarted is not null;

	[MemberNotNullWhen(true, nameof(TimeEnded))]
	public bool HasEnded => TimeEnded is not null;

	public DateOnly GetEffectiveDate() => TimeStarted?.Date.ToDateOnly() ?? CreatedAt.Date.ToDateOnly();

	public string GetTitle(bool includeDate = true)
	{
		var sb = new StringBuilder();
		if (!HasStarted)
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

	public string? GetFormattedDuration()
	{
		if (!HasStarted)
			return null;

		var endTime = TimeEnded ?? DateTimeOffset.Now;
		var duration = endTime - TimeStarted.Value;

		if (duration.TotalMinutes < 1)
			return "Less than a minute";
		if (duration.TotalDays >= 1)
			return "Over a day";

		var sb = new StringBuilder();

		if (duration.Hours > 0)
			sb.Append($"{duration.Hours}h ");

		if (duration.Minutes > 0)
			sb.Append($"{duration.Minutes}m ");

		// Trim last space
		if (sb.Length > 0)
			sb.Length--;

		return sb.ToString();
	}
}