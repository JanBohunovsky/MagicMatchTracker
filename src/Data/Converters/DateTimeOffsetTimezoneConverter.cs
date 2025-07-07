using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MagicMatchTracker.Data.Converters;

/// <summary>
/// Converts the timezone of <see cref="DateTimeOffset"/> values to UTC for the database, and to local timezone from the database.
/// </summary>
[UsedImplicitly]
public sealed class DateTimeOffsetTimezoneConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
	public DateTimeOffsetTimezoneConverter() : base(t => t.ToUniversalTime(), t => t.ToLocalTime()) { }
}