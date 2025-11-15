using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MagicMatchTracker.Data.Converters;

[UsedImplicitly]
public sealed class MatchEventDataJsonConverter() : ValueConverter<MatchEventData?, string?>(
	data => data != null ? JsonSerializer.Serialize(data, JsonSerializerOptions) : null,
	json => json != null ? JsonSerializer.Deserialize<MatchEventData?>(json, JsonSerializerOptions) : null)
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		WriteIndented = false,
	};
}