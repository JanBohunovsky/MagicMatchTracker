using System.Diagnostics.CodeAnalysis;
using System.Text;
using Humanizer;

namespace MagicMatchTracker.Data.Models;

public sealed class Match : IEntity
{
	public Guid Id { get; private init; }

	public DateTimeOffset? TimeStarted { get; set; }

	public DateTimeOffset? TimeEnded { get; set; }

	public int MatchNumber { get; set; }

	public bool IsLive { get; set; } = true;

	public string? Notes { get; set; }

	public List<MatchParticipation> Participations { get; set; } = [];

	[MemberNotNullWhen(true, nameof(TimeStarted))]
	public bool HasStarted => TimeStarted is not null;

	[MemberNotNullWhen(true, nameof(TimeEnded))]
	public bool HasEnded => TimeEnded is not null;

	public bool IsInProgress => HasStarted && !HasEnded;

	public DateOnly GetEffectiveDate() => TimeStarted?.Date.ToDateOnly() ?? DateOnly.Today;

	public static DateTimeOffset GetDateTimeForNonLiveMatch(DateOnly date)
	{
		return new DateTimeOffset(date, new TimeOnly(12, 00), TimeSpan.Zero);
	}

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
		if (date.Year < DateOnly.Today.Year)
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
		if (!HasStarted || !IsLive)
			return null;

		var endTime = TimeEnded ?? DateTimeOffset.Now;
		var duration = endTime - TimeStarted.Value;
		if (duration <= TimeSpan.Zero)
			return null;

		return duration.Humanize(precision: 2, maxUnit: TimeUnit.Day, minUnit: TimeUnit.Minute, collectionSeparator: " ");
	}

	public int? GetTotalTurns()
	{
		return Participations
			.Choose(mp => mp.EndState)
			.Select(e => e.Turn)
			.DefaultIfEmpty(null)
			.Max();
	}
}